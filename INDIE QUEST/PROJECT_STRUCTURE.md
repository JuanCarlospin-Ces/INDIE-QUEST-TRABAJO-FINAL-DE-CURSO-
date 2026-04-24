# Indie Quest — Estructura del Proyecto

> **Indie Quest** es una plataforma social para desarrolladores de videojuegos independientes. La API permite gestionar usuarios y publicaciones (*posts*), y está diseñada como una **API REST** siguiendo la **Arquitectura Hexagonal** combinada con el patrón **CQRS**.

---

## Índice

1. [Visión general de la solución](#1-visión-general-de-la-solución)
2. [Tipo de API — REST](#2-tipo-de-api--rest)
3. [Arquitectura Hexagonal](#3-arquitectura-hexagonal)
4. [IndieQuest-Api](#4-indiequest-api--implementación-en-memoria)
5. [IndieQuest-Api\_with\_PosGre](#5-indiequest-api_with_posgre--implementación-postgresql)
6. [IndieQuest-Test](#6-indiequest-test--suite-de-pruebas)
7. [IndieQuest-DataBase](#7-indiequest-database--base-de-datos)
8. [Markdowns](#8-markdowns--documentación-técnica)
9. [Diagrama de relaciones de datos](#9-diagrama-de-relaciones-de-datos)

---

## 1. Visión general de la solución

```
INDIE QUEST.sln
├── IndieQuest-Api/                  → API con persistencia en memoria (In-Memory)
├── IndieQuest-Api_with_PosGre/      → API con persistencia en PostgreSQL (EF Core)
├── IndieQuest-Test/                 → Suite de pruebas (Unit, Integration, Acceptance, E2E)
├── IndieQuest-DataBase/             → Scripts SQL de esquema y datos iniciales
├── Markdowns/                       → Documentación técnica adicional
└── PROJECT_STRUCTURE.md             → Este archivo
```

La solución contiene **dos variantes de la API** que comparten exactamente la misma arquitectura y contratos de dominio. La diferencia única es la capa de infraestructura: una usa colecciones en memoria y la otra usa PostgreSQL a través de Entity Framework Core. Esto demuestra cómo la Arquitectura Hexagonal permite **sustituir la tecnología de persistencia sin tocar el dominio ni la aplicación**.

---

## 2. Tipo de API — REST

Indie Quest expone una **API REST** (*Representational State Transfer*), el estilo arquitectónico más extendido para servicios web HTTP.

### Confirmación del estilo REST

La API cumple todos los rasgos que definen REST:

| Característica REST          | Implementación en Indie Quest                                              |
|------------------------------|----------------------------------------------------------------------------|
| **Interfaz uniforme**        | Recursos identificados por URI: `api/user`, `api/post`, `api/post/{id}`    |
| **Verbos HTTP semánticos**   | `GET` (leer), `POST` (crear), `PUT` (actualizar), `DELETE` (eliminar)      |
| **Sin estado (stateless)**   | Cada petición es independiente; no hay sesión en el servidor               |
| **Representación en JSON**   | Todas las respuestas y cuerpos de petición usan JSON                       |
| **Códigos de estado HTTP**   | `200 OK`, `404 Not Found` utilizados correctamente                         |
| **Sistema por capas**        | El cliente solo interactúa con los controladores; la infraestructura es opaca|

### Nivel de madurez REST (Richardson Maturity Model)

El modelo de Richardson define 4 niveles de adopción de REST:

```
Nivel 0 — Un único endpoint, todas las operaciones en el mismo URI
Nivel 1 — Recursos individuales (URIs diferenciadas por recurso)
Nivel 2 — Verbos HTTP + códigos de estado  ← Indie Quest opera aquí
Nivel 3 — HATEOAS (hipervínculos en las respuestas)
```

Indie Quest implementa el **Nivel 2**, que es el estándar de facto en la industria. El Nivel 3 (HATEOAS) añadiría enlaces de navegación en cada respuesta JSON para que el cliente descubra las acciones disponibles, pero no está implementado, lo cual es habitual en APIs orientadas a un frontend conocido.

### Ejemplo de interacción REST

```http
# Crear un usuario
POST /api/user
Content-Type: application/json

{ "username": "nuevo_dev", "email": "dev@example.com", "password": "1234" }

# Respuesta
HTTP/1.1 200 OK

# Obtener todos los posts de un usuario
GET /api/post/user/abc-123

# Respuesta
HTTP/1.1 200 OK
Content-Type: application/json
[ { "postId": "...", "title": "...", ... } ]

# Eliminar un post
DELETE /api/post/abc-456

# Respuesta
HTTP/1.1 200 OK
```

---

## 3. Arquitectura Hexagonal

La Arquitectura Hexagonal (también conocida como *Ports & Adapters*, propuesta por Alistair Cockburn) organiza el software en capas concéntricas donde **el dominio es el núcleo y nunca depende del exterior**.

### Principio fundamental

> Las capas internas no conocen a las capas externas. La comunicación hacia el interior se realiza a través de **puertos** (interfaces); las implementaciones concretas son **adaptadores** que se conectan a esos puertos desde el exterior.

### Capas implementadas en Indie Quest

```
┌──────────────────────────────────────────────────────────┐
│              CAPA DE PRESENTACIÓN (HTTP)                 │
│          Controllers  ──  Adaptador de entrada           │
│      (UserController, PostController)                    │
└─────────────────────────┬────────────────────────────────┘
                          │  invoca
┌─────────────────────────▼────────────────────────────────┐
│             CAPA DE APLICACIÓN (CQRS)                    │
│   Command Handlers ──────────── Query Handlers           │
│  (Create / Update / Delete)   (GetAll / GetById / ...)   │
│                                                          │
│   Orquesta el flujo sin lógica de negocio propia.        │
│   Sólo coordina Dominio e Infraestructura.               │
└─────────────────────────┬────────────────────────────────┘
                          │  usa puertos (interfaces)
┌─────────────────────────▼────────────────────────────────┐
│              CAPA DE DOMINIO (Núcleo)                    │
│   Modelos: User, Post, Tag, UserPost, PostTag            │
│   Puertos:  IUserRepository, IPostRepository             │
│                                                          │
│   ✦ No depende de ninguna otra capa.                     │
│   ✦ Define QUÉ se puede hacer, no CÓMO.                  │
└─────────────────────────┬────────────────────────────────┘
                          │  implementado por adaptadores
┌─────────────────────────▼────────────────────────────────┐
│           CAPA DE INFRAESTRUCTURA (Adaptadores)          │
│                                                          │
│  ┌─────────────────────┐   ┌──────────────────────────┐  │
│  │  InMemoryRepository │   │  PostgreSqlRepository    │  │
│  │  (colecciones C#)   │   │  (EF Core + Npgsql)      │  │
│  └─────────────────────┘   └──────────────────────────┘  │
└──────────────────────────────────────────────────────────┘
```

### Los Puertos (Ports)

Los puertos son las **interfaces definidas en el dominio** que abstraen la persistencia:

| Puerto              | Ubicación                              | Métodos principales                                                           |
|---------------------|----------------------------------------|-------------------------------------------------------------------------------|
| `IUserRepository`   | `Domain/Repository/IUserRepository.cs` | `GetAllUsersAsync`, `GetUserByIdAsync`, `CreateUserAsync`, `UpdateUserAsync`, `DeleteUserAsync` |
| `IPostRepository`   | `Domain/Repository/IPostRepository.cs` | `GetAllPostsAsync`, `GetPostByIdAsync`, `GetPostsByUserIdAsync`, `CreatePostAsync`, `UpdatePostAsync`, `DeletePostAsync` |

### Los Adaptadores (Adapters)

Los adaptadores son las **implementaciones concretas** de esos puertos. Existen dos adaptadores de salida por repositorio:

| Adaptador                     | Proyecto                    | Tecnología          |
|-------------------------------|-----------------------------|---------------------|
| `InMemoryUserRepository`      | `IndieQuest-Api`            | `List<User>` en RAM |
| `InMemoryPostRepository`      | `IndieQuest-Api`            | `List<Post>` en RAM |
| `PostgreSqlUserRepository`    | `IndieQuest-Api_with_PosGre`| EF Core + PostgreSQL|
| `PostgreSqlPostRepository`    | `IndieQuest-Api_with_PosGre`| EF Core + PostgreSQL|

### El patrón CQRS en la capa de aplicación

La capa de aplicación implementa **CQRS (Command Query Responsibility Segregation)**, separando estrictamente las operaciones de escritura (*Commands*) de las de lectura (*Queries*):

```
Application/
├── Command/
│   ├── Users/
│   │   ├── CreateUserCommand.cs          ← DTO con los datos de entrada
│   │   ├── CreateUserCommandHandler.cs   ← Lógica de orquestación
│   │   ├── UpdateUserCommand.cs
│   │   ├── UpdateUserCommandHandler.cs
│   │   └── DeleteUserCommandHandler.cs
│   └── Posts/
│       ├── CreatePostCommand.cs
│       ├── CreatePostCommandHandler.cs
│       ├── UpdatePostCommand.cs
│       ├── UpdatePostCommandHandler.cs
│       └── DeletePostCommandHandler.cs
└── Queries/
    ├── GetAllUsers/
    │   └── GetAllUsersQueryHandler.cs
    ├── GetUserById/
    │   └── GetUserByIdQueryHandler.cs
    ├── GetAllPosts/
    │   └── GetAllPostsQueryHandler.cs
    ├── GetPostById/
    │   └── GetPostByIdQueryHandler.cs
    └── GetPostsByUserId/
        └── GetPostsByUserIdQueryHandler.cs
```

Cada handler recibe el repositorio (puerto) por inyección de dependencias y lo invoca sin saber qué adaptador concreto hay detrás:

```csharp
// CreateUserCommandHandler.cs — depende sólo del puerto IUserRepository
public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;   // Puerto (interfaz)

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;               // Adaptador inyectado en Program.cs
    }

    public async Task Handle(CreateUserCommand command)
    {
        var user = new User { ... };
        await _userRepository.CreateUserAsync(user);
    }
}
```

### Inyección de dependencias — el pegamento de los adaptadores

En `Program.cs` se registra **qué adaptador** se conecta a cada puerto:

```csharp
// IndieQuest-Api → adaptador In-Memory
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();

// IndieQuest-Api_with_PosGre → adaptador PostgreSQL
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
builder.Services.AddScoped<IPostRepository, PostgreSqlPostRepository>();
```

Cambiar de una tecnología de persistencia a otra **no requiere modificar** ningún handler, ningún modelo de dominio ni ningún controlador. Solo se cambia el registro en `Program.cs`.

---

## 4. IndieQuest-Api — Implementación en Memoria

Implementación completa de la API con almacenamiento en memoria. Ideal para desarrollo, pruebas locales y demostración sin dependencias externas.

### Estructura de archivos

```
IndieQuest-Api/
├── Program.cs                          → Punto de entrada, DI, Swagger, CORS
├── IQ-Api.csproj                       → .NET 10.0 | Swashbuckle.AspNetCore 10.1.7
├── appsettings.json                    → Configuración de logging
├── appsettings.Development.json        → Configuración de desarrollo
├── Properties/
│   └── launchSettings.json             → HTTP :5063 / HTTPS :7058
│
├── Controllers/
│   ├── UserController.cs               → Endpoints REST para usuarios  (api/user)
│   └── PostController.cs               → Endpoints REST para posts     (api/post)
│
├── Domain/
│   ├── Model/
│   │   ├── User.cs                     → Entidad de usuario
│   │   └── Post.cs                     → Entidad de post
│   ├── ValueObject/
│   │   └── Tag.cs                      → Objeto de valor Tag
│   └── Repository/
│       ├── IUserRepository.cs          → Puerto de persistencia de usuarios
│       └── IPostRepository.cs          → Puerto de persistencia de posts
│
├── Application/
│   ├── Command/
│   │   ├── Users/
│   │   │   ├── CreateUserCommand.cs
│   │   │   ├── CreateUserCommandHandler.cs
│   │   │   ├── UpdateUserCommand.cs
│   │   │   ├── UpdateUserCommandHandler.cs
│   │   │   └── DeleteUserCommandHandler.cs
│   │   └── Posts/
│   │       ├── CreatePostCommand.cs
│   │       ├── CreatePostCommandHandler.cs
│   │       ├── UpdatePostCommand.cs
│   │       ├── UpdatePostCommandHandler.cs
│   │       └── DeletePostCommandHandler.cs
│   └── Queries/
│       ├── GetAllUsers/GetAllUsersQueryHandler.cs
│       ├── GetUserById/GetUserByIdQueryHandler.cs
│       ├── GetAllPosts/GetAllPostsQueryHandler.cs
│       ├── GetPostById/GetPostByIdQueryHandler.cs
│       └── GetPostsByUserId/GetPostsByUserIdQueryHandler.cs
│
└── Infrastructure/
    └── Repository/
        └── InMemory/
            ├── InMemoryUserRepository.cs   → Adaptador: List<User> con 3 usuarios precargados
            └── InMemoryPostRepository.cs   → Adaptador: List<Post> con 3 posts precargados
```

### Modelos de dominio

#### `User`
| Propiedad              | Tipo       | Descripción                          |
|------------------------|------------|--------------------------------------|
| `UserId`               | `string`   | Identificador único (GUID string)    |
| `Username`             | `string`   | Nombre de usuario                    |
| `Password`             | `string`   | Contraseña                           |
| `Email`                | `string`   | Correo electrónico                   |
| `AvailableForWork`     | `bool?`    | Disponibilidad para colaborar        |
| `UserBio`              | `string?`  | Biografía del usuario                |
| `UserProfilePicture`   | `string?`  | Ruta o URL de la foto de perfil      |
| `dateOfRegistration`   | `DateTime` | Fecha de alta (UTC)                  |

#### `Post`
| Propiedad        | Tipo       | Descripción                          |
|------------------|------------|--------------------------------------|
| `PostId`         | `string`   | Identificador único (GUID string)    |
| `PostUserId`     | `string`   | ID del usuario autor                 |
| `Title`          | `string`   | Título del post                      |
| `MediaContent`   | `string`   | Ruta o URL del contenido multimedia  |
| `Description`    | `string?`  | Descripción del post                 |
| `CreationDate`   | `DateTime` | Fecha de creación (UTC)              |
| `Tags`           | `Tag[]?`   | Etiquetas asociadas                  |

#### `Tag` (Value Object)
| Propiedad   | Tipo     | Descripción         |
|-------------|----------|---------------------|
| `tagId`     | `string` | Identificador       |
| `tagName`   | `string` | Nombre de la etiqueta|

### Endpoints expuestos

#### Usuarios — `api/user`

| Método   | Ruta             | Handler                        | Descripción              |
|----------|------------------|--------------------------------|--------------------------|
| `GET`    | `/api/user`      | `GetAllUsersQueryHandler`      | Obtener todos los usuarios|
| `GET`    | `/api/user/{id}` | `GetUserByIdQueryHandler`      | Obtener usuario por ID   |
| `POST`   | `/api/user`      | `CreateUserCommandHandler`     | Crear usuario            |
| `PUT`    | `/api/user/{id}` | `UpdateUserCommandHandler`     | Actualizar usuario       |
| `DELETE` | `/api/user/{id}` | `DeleteUserCommandHandler`     | Eliminar usuario         |

#### Posts — `api/post`

| Método   | Ruta                         | Handler                           | Descripción               |
|----------|------------------------------|-----------------------------------|---------------------------|
| `GET`    | `/api/post`                  | `GetAllPostsQueryHandler`         | Obtener todos los posts   |
| `GET`    | `/api/post/{id}`             | `GetPostByIdQueryHandler`         | Obtener post por ID       |
| `GET`    | `/api/post/user/{userId}`    | `GetPostsByUserIdQueryHandler`    | Posts de un usuario       |
| `POST`   | `/api/post`                  | `CreatePostCommandHandler`        | Crear post                |
| `PUT`    | `/api/post/{id}`             | `UpdatePostCommandHandler`        | Actualizar post           |
| `DELETE` | `/api/post/{id}`             | `DeletePostCommandHandler`        | Eliminar post             |

### Dependencias NuGet

| Paquete                       | Versión   | Uso                  |
|-------------------------------|-----------|----------------------|
| `Swashbuckle.AspNetCore`      | 10.1.7    | Documentación Swagger|

---

## 5. IndieQuest-Api\_with\_PosGre — Implementación PostgreSQL

Variante de la API con persistencia real en PostgreSQL mediante **Entity Framework Core**. Comparte la misma arquitectura y contratos que la versión en memoria; únicamente difiere la capa de infraestructura.

### Cambios respecto a la versión en memoria

| Aspecto               | `IndieQuest-Api`          | `IndieQuest-Api_with_PosGre`         |
|-----------------------|---------------------------|--------------------------------------|
| Tipo de ID            | `string` (GUID)           | `int` (autoincremental DB)           |
| Persistencia          | `List<T>` en RAM          | PostgreSQL (tablas reales)           |
| Repositorios          | `InMemory*Repository`     | `PostgreSql*Repository` (EF Core)    |
| DbContext             | —                         | `IndieQuestDbContext`                |
| Relaciones N:M        | Array embebido            | Tablas intermedias (`UserPost`, `PostTag`) |
| Ciclo de vida DI      | `AddSingleton`            | `AddScoped`                          |

### Estructura de archivos

```
IndieQuest-Api_with_PosGre/
├── Program.cs                          → DI con PostgreSQL, DbContext, CORS, Swagger
├── IQ-Api.csproj                       → EF Core 10.0.0 + Npgsql 10.0.0
├── appsettings.json                    → ConnectionString PostgreSQL
├── appsettings.Development.json        → Configuración de desarrollo
├── Properties/
│   └── launchSettings.json
│
├── Controllers/
│   ├── UserController.cs               → Mismo contrato REST (parámetros int)
│   └── PostController.cs               → Mismo contrato REST (parámetros int)
│
├── Domain/
│   ├── Model/
│   │   ├── User.cs                     → Entidad con int UserId + navegación UserPosts
│   │   └── Post.cs                     → Entidad con int PostId + navegación UserPosts/PostTags
│   ├── ValueObject/
│   │   ├── Tag.cs                      → int tagId + navegación PostTags
│   │   ├── UserPost.cs                 → Tabla intermedia User ↔ Post
│   │   └── PostTag.cs                  → Tabla intermedia Post ↔ Tag
│   └── Repository/
│       ├── IUserRepository.cs          → Puerto (int userId)
│       └── IPostRepository.cs          → Puerto (int postId)
│
├── Application/
│   ├── Command/                        → Misma estructura que IndieQuest-Api (ids: int)
│   └── Queries/                        → Misma estructura que IndieQuest-Api (ids: int)
│
└── Infrastructure/
    ├── IndieQuestDbContext.cs           → Configuración EF Core (mapeo a tablas SQL)
    └── Repository/
        └── PostgreSQL/
            ├── PostgreSqlUserRepository.cs  → Adaptador EF Core para usuarios
            └── PostgreSqlPostRepository.cs  → Adaptador EF Core para posts
```

### Modelos de dominio (diferencias con la versión en memoria)

#### `User` (PostgreSQL)
```csharp
public class User {
    public int UserId { get; set; }                          // int autoincremental
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool? AvailableForWork { get; set; }
    public string? UserBio { get; set; }
    public string? UserProfilePicture { get; set; }
    public required string Email { get; set; }
    public DateTime dateOfRegistration { get; set; }
    public ICollection<UserPost> UserPosts { get; set; }     // Navegación N:M
}
```

#### `UserPost` (tabla intermedia `Makes_MadeBy`)
```csharp
public class UserPost {
    public int UserId { get; set; }
    public User User { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}
```

#### `PostTag` (tabla intermedia `Has_Tag`)
```csharp
public class PostTag {
    public int PostId { get; set; }
    public Post Post { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}
```

### `IndieQuestDbContext`

El DbContext centraliza la configuración del mapeo entre entidades C# y tablas PostgreSQL:

| Entidad    | Tabla SQL         | Clave primaria     | Relaciones configuradas              |
|------------|-------------------|--------------------|--------------------------------------|
| `User`     | `"User"`          | `idUser`           | 1:N con `UserPost` (Cascade delete)  |
| `Post`     | `Post`            | `idPost`           | 1:N con `UserPost`, 1:N con `PostTag`|
| `Tag`      | `Tag`             | `idTag`            | 1:N con `PostTag`                    |
| `UserPost` | `Makes_MadeBy`    | `(idUser, idPost)` | FK a User y Post                     |
| `PostTag`  | `Has_Tag`         | `(idPost, idTag)`  | FK a Post y Tag                      |

### Cadena de conexión

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=Indie_Quest_DB;User Id=postgres;Password=jc123;"
  }
}
```

### Dependencias NuGet adicionales

| Paquete                                    | Versión  | Uso                          |
|--------------------------------------------|----------|------------------------------|
| `Microsoft.EntityFrameworkCore`            | 10.0.0   | ORM base                     |
| `Npgsql.EntityFrameworkCore.PostgreSQL`    | 10.0.0   | Proveedor PostgreSQL para EF |
| `Swashbuckle.AspNetCore`                   | 10.1.7   | Documentación Swagger        |

---

## 6. IndieQuest-Test — Suite de Pruebas

Proyecto de pruebas que cubre la API en memoria (`IndieQuest-Api`) en cuatro niveles de la pirámide de testing.

### Estructura de archivos

```
IndieQuest-Test/
├── IndieQuest.Test.csproj              → NUnit 4.2.2 + Moq 4.20.72 + MVC.Testing
│
├── UnitTest/                           → Pruebas de Query Handlers en aislamiento
│   ├── GetUserByIdQueryHandlerTests.cs
│   ├── GetPostByIdQueryHandlerTests.cs
│   └── GetPostsByUserIdQueryHandlerTests.cs
│
├── IntegrationTest/                    → Pruebas de repositorios en memoria
│   ├── InMemoryUserRepositoryCreateUserTests.cs
│   ├── InMemoryUserRepositoryGetAllUsersTests.cs
│   ├── InMemoryUserRepositoryUpdateUserTests.cs
│   ├── InMemoryUserRepositoryDeleteUserTests.cs
│   ├── InMemoryPostRepositoryCreatePostTests.cs
│   ├── InMemoryPostRepositoryGetAllPostsTests.cs
│   ├── InMemoryPostRepositoryUpdatePostTests.cs
│   └── InMemoryPostRepositoryDeletePostTests.cs
│
├── AcceptanceTest/                     → Pruebas de Command/Query Handlers con mocks
│   ├── CreateUserCommandHandlerTests.cs
│   ├── UpdateUserCommandHandlerTests.cs
│   ├── DeleteUserCommandHandlerTests.cs
│   ├── GetAllUsersQueryHandlerTests.cs
│   ├── CreatePostCommandHandlerTests.cs
│   ├── UpdatePostCommandHandlerTests.cs
│   ├── DeletePostCommandHandlerTests.cs
│   └── GetAllPostsQueryHandlerTests.cs
│
└── EndToEndTest/                       → Pruebas de Controllers vía HTTP
    ├── UserControllerCreateUserTests.cs
    ├── UserControllerGetAllUsersTests.cs
    ├── UserControllerGetUserByIdTests.cs
    ├── UserControllerUpdateUserTests.cs
    ├── UserControllerDeleteUserTests.cs
    ├── PostControllerCreatePostTests.cs
    ├── PostControllerGetAllPostsTests.cs
    ├── PostControllerGetPostByIdTests.cs
    ├── PostControllerGetPostsByUserIdTests.cs
    ├── PostControllerUpdatePostTests.cs
    └── PostControllerDeletePostTests.cs
```

### Niveles de prueba

#### Unit Tests — Aislamiento total

Prueban un único handler de query inyectando un mock del repositorio. No hay dependencias externas.

- Tecnología: **NUnit** + **Moq**
- Alcance: `GetUserByIdQueryHandler`, `GetPostByIdQueryHandler`, `GetPostsByUserIdQueryHandler`
- Patrón: Arrange (mock) → Act (handler.Handle) → Assert (resultado)

#### Integration Tests — Repositorio real en memoria

Prueban la implementación `InMemoryRepository` directamente, sin mocks. Verifican que la lógica de acceso a datos funciona correctamente.

- Tecnología: **NUnit**
- Alcance: `InMemoryUserRepository`, `InMemoryPostRepository`
- Cobertura: Create, GetAll, Update, Delete para usuarios y posts

#### Acceptance Tests — Handlers con mocks de repositorio

Prueban los Command Handlers y los Query Handlers de forma aislada, usando Moq para simular el repositorio. Validan la lógica de orquestación de la capa de aplicación.

- Tecnología: **NUnit** + **Moq**
- Alcance: todos los Command Handlers y Query Handlers de colección
- Cobertura: operaciones CRUD para usuarios y posts

#### End-to-End Tests — Pila HTTP completa

Prueban los controladores REST usando `WebApplicationFactory`, enviando peticiones HTTP reales y verificando respuestas (status code, cuerpo JSON).

- Tecnología: **NUnit** + **Microsoft.AspNetCore.Mvc.Testing**
- Alcance: `UserController`, `PostController`
- Cobertura: todos los endpoints (5 para usuarios, 6 para posts)

### Dependencias NuGet

| Paquete                                   | Versión   | Uso                              |
|-------------------------------------------|-----------|----------------------------------|
| `NUnit`                                   | 4.2.2     | Framework de testing             |
| `NUnit3TestAdapter`                       | 4.6.0     | Integración con test runner      |
| `Microsoft.NET.Test.Sdk`                  | 17.12.0   | SDK de pruebas .NET              |
| `Moq`                                     | 4.20.72   | Mocking de dependencias          |
| `Microsoft.AspNetCore.Mvc.Testing`        | 10.0.0    | Testing de controladores HTTP    |

---

## 7. IndieQuest-DataBase — Base de Datos

Scripts SQL para la creación del esquema y la carga de datos iniciales en PostgreSQL.

### Estructura de archivos

```
IndieQuest-DataBase/
├── IQ-DB.sql                   → Creación del esquema (tablas, PKs, FKs)
└── IQ-StartingEntities.sql     → Inserción de datos de ejemplo
```

### Esquema de la base de datos (`IQ-DB.sql`)

```sql
-- Tabla principal de usuarios
"User" (idUser SERIAL PK, userName, email, password,
        ProfilePicture, userBio, availableForWork, dateOfRegistration)

-- Tabla de publicaciones
Post (idPost SERIAL PK, postTitle, mediaContent,
      Description, CreationDate)

-- Catálogo de etiquetas
Tag (idTag SERIAL PK, tagName)

-- Relación N:M entre User y Post
Makes_MadeBy (idUser FK → "User".idUser,
              idPost FK → Post.idPost,
              PK compuesta)

-- Relación N:M entre Post y Tag
Has_Tag (idPost FK → Post.idPost,
         idTag  FK → Tag.idTag,
         PK compuesta)
```

### Datos de ejemplo (`IQ-StartingEntities.sql`)

| Entidad | Registros insertados                                         |
|---------|--------------------------------------------------------------|
| Usuarios| `john_doe`, `jane_smith`, `alice_wonder`                     |
| Posts   | "First Post", "Second Post", "Third Post" (1 por usuario)    |
| Tags    | `Software`, `Design`, `Content`                              |
| Relaciones| Cada post vinculado a su usuario y a su tag correspondiente|

---

## 8. Markdowns — Documentación Técnica

```
Markdowns/
├── LOCAL_MEDIA_STORAGE_SETUP.md        → Guía de almacenamiento local de media
└── POSTGRESQL_MIGRATION_GUIDE.md       → Guía de migración In-Memory → PostgreSQL
```

### `LOCAL_MEDIA_STORAGE_SETUP.md`

Documenta la estrategia de almacenamiento local para ficheros multimedia (fotos de perfil y contenido de posts). Define la estructura de directorios:

```
IndieQuest-LocalData/
├── user/
│   ├── 1/    → Archivos de perfil de john_doe
│   ├── 2/    → Archivos de perfil de jane_smith
│   └── 3/    → Archivos de perfil de alice_wonder
└── postdata/
    ├── 1/    → Media del "First Post"
    ├── 2/    → Media del "Second Post"
    └── 3/    → Media del "Third Post"
```

Incluye el flujo de subida de archivos, configuración de archivos estáticos en ASP.NET Core y consideraciones de seguridad.

### `POSTGRESQL_MIGRATION_GUIDE.md`

Guía paso a paso para migrar la implementación en memoria a PostgreSQL. Cubre:
- Actualización de modelos (de `string` a `int` como tipo de ID)
- Implementación del patrón repositorio con EF Core
- Configuración del `DbContext` con mapeo de entidades
- Modelado de relaciones N:M mediante tablas intermedias
- Cambios en Commands, Handlers y Controllers
- Instalación de paquetes NuGet necesarios
- Ejemplos de implementación de repositorios PostgreSQL

---

## 9. Diagrama de relaciones de datos

```
                    ┌───────────┐
                    │   User    │
                    │──────────-│
                    │ idUser PK │
                    │ userName  │
                    │ email     │
                    │ password  │
                    │ userBio   │
                    │ available │
                    │ regDate   │
                    └─────┬─────┘
                          │ 1
                          │
                          │ N
                    ┌─────▼──────────┐
                    │ Makes_MadeBy   │
                    │────────────────│
                    │ idUser FK  PK  │
                    │ idPost FK  PK  │
                    └─────┬──────────┘
                          │ N
                          │
                          │ 1
                    ┌─────▼─────┐         ┌─────────────┐
                    │   Post    │         │     Tag     │
                    │───────────│         │─────────────│
                    │ idPost PK │    N    │ idTag PK    │
                    │ postTitle │◄───────►│ tagName     │
                    │ media     │  Has_Tag│             │
                    │ descr.    │         └─────────────┘
                    │ date      │
                    └───────────┘
```

**Cardinalidades:**
- Un **User** puede tener múltiples **Posts** (1:N a través de `Makes_MadeBy`)
- Un **Post** puede tener múltiples **Tags** y un **Tag** puede aplicarse a múltiples **Posts** (N:M a través de `Has_Tag`)

---

## Resumen de tecnologías

| Categoría            | Tecnología / Herramienta                        |
|----------------------|-------------------------------------------------|
| Framework            | ASP.NET Core 10.0 (Web API)                     |
| ORM                  | Entity Framework Core 10.0 (versión PostgreSQL) |
| Base de datos        | PostgreSQL 16+ con Npgsql                       |
| Documentación API    | Swagger / Swashbuckle 10.1.7                    |
| Testing              | NUnit 4.2.2 + Moq 4.20.72                       |
| Testing HTTP         | Microsoft.AspNetCore.Mvc.Testing 10.0.0         |
| Estilo de API        | REST (Richardson Maturity Level 2)              |
| Arquitectura         | Hexagonal (Ports & Adapters) + CQRS             |
| Lenguaje             | C# 13 / .NET 10.0                               |
