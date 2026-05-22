# Indie Quest — Estructura del Proyecto

> **Indie Quest** es una plataforma social para desarrolladores de videojuegos independientes. La API permite gestionar usuarios y publicaciones (*posts*), y está diseñada como una **API REST** siguiendo la **Arquitectura Hexagonal** combinada con el patrón **CQRS**.

---

## Índice

1. [Visión general de la solución](#1-visión-general-de-la-solución)
2. [Tipo de API — REST](#2-tipo-de-api--rest)
3. [Arquitectura Hexagonal](#3-arquitectura-hexagonal)
4. [IndieQuest-Api](#4-indiequest-api--implementación-net-10)
5. [IndieQuest-Tests](#5-indiequest-tests--suite-de-pruebas)
6. [IndieQuest-DataBase](#6-indiequest-database--base-de-datos)
7. [IndieQuest-UI](#7-indiequest-ui--frontend-principal)
8. [IndieQuest-AdminInterface](#8-indiequest-admininterface--panel-de-administración)
9. [IndieQuest-LocalData](#9-indiequest-localdata--almacenamiento-multimedia)
10. [Diagrama de relaciones de datos](#10-diagrama-de-relaciones-de-datos)

---

## 1. Visión general de la solución

```
INDIE QUEST.sln
├── IndieQuest-Api/                  → API REST con .NET 10.0 (persistencia PostgreSQL)
├── IndieQuest-Tests/                → Suite de pruebas (Unit, Integration, Acceptance, E2E)
├── IndieQuest-DataBase/             → Scripts SQL de esquema y datos iniciales
├── IndieQuest-UI/                   → Frontend principal (React + Vite)
├── IndieQuest-AdminInterface/       → Panel de administración (React + Vite)
├── IndieQuest-LocalData/            → Almacenamiento local de contenido multimedia
└── PROJECT_STRUCTURE.md             → Este archivo
```

La solución implementa una arquitectura moderna de **microservicios frontend** con una **API REST centralizada**. La API utiliza **.NET 10.0** con **Entity Framework Core** y **PostgreSQL**, implementando la **Arquitectura Hexagonal** combinada con el patrón **CQRS** para garantizar escalabilidad, mantenibilidad y separación de responsabilidades.

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
│  ┌──────────────────────────────────────────────────┐   │
│  │  PostgreSqlRepository (EF Core + Npgsql)         │   │
│  │  ├── PostgreSqlUserRepository                    │   │
│  │  └── PostgreSqlPostRepository                    │   │
│  └──────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────┘
```

### Los Puertos (Ports)

Los puertos son las **interfaces definidas en el dominio** que abstraen la persistencia:

| Puerto              | Ubicación                              | Métodos principales                                                           |
|---------------------|----------------------------------------|-------------------------------------------------------------------------------|
| `IUserRepository`   | `Domain/Repository/IUserRepository.cs` | `GetAllUsersAsync`, `GetUserByIdAsync`, `CreateUserAsync`, `UpdateUserAsync`, `DeleteUserAsync` |
| `IPostRepository`   | `Domain/Repository/IPostRepository.cs` | `GetAllPostsAsync`, `GetPostByIdAsync`, `GetPostsByUserIdAsync`, `CreatePostAsync`, `UpdatePostAsync`, `DeletePostAsync` |

### Los Adaptadores (Adapters)

Los adaptadores son las **implementaciones concretas** de esos puertos utilizados en el proyecto:

| Adaptador                     | Ubicación                  | Tecnología          |
|-------------------------------|----------------------------|---------------------|
| `PostgreSqlUserRepository`    | `Infrastructure/Repository/PostgreSQL/` | EF Core + PostgreSQL|
| `PostgreSqlPostRepository`    | `Infrastructure/Repository/PostgreSQL/` | EF Core + PostgreSQL|

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
// IndieQuest-Api → Adaptador PostgreSQL
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
builder.Services.AddScoped<IPostRepository, PostgreSqlPostRepository>();
```

Cambiar la implementación del repositorio (ej: a otra BD) **no requiere modificar** ningún handler, modelo o controlador. Solo se cambia el registro en `Program.cs`.

---

## 4. IndieQuest-Api — Implementación .NET 10

API REST con persistencia real en PostgreSQL mediante **Entity Framework Core 10.0.0**. Implementa **Arquitectura Hexagonal** con patrón **CQRS** para máxima escalabilidad y mantenibilidad.

### Estructura de archivos

```
IndieQuest-Api/
├── Program.cs                          → Punto de entrada, DI, Swagger, CORS, middleware
├── IQ-Api.csproj                       → .NET 10.0 | EF Core 10.0.0 | Npgsql 10.0.0 | Swashbuckle 10.1.7
├── appsettings.json                    → Configuración (connection string PostgreSQL, logging)
├── appsettings.Development.json        → Configuración de desarrollo
├── Dockerfile                          → Imagen Docker multi-stage para producción
├── Properties/
│   └── launchSettings.json             → HTTP :5063 / HTTPS :7058
│
├── Controllers/
│   ├── UserController.cs               → Endpoints REST para usuarios  (api/users)
│   │   ├── GET    /api/users           → GetAllUsers (paginado, filtro availableForWork)
│   │   ├── GET    /api/users/{id}      → GetUserById
│   │   ├── POST   /api/users           → CreateUser
│   │   ├── PUT    /api/users/{id}      → UpdateUser
│   │   ├── DELETE /api/users/{id}      → DeleteUser
│   │   ├── POST   /api/users/{id}/picture → UploadProfilePicture (multipart/form-data)
│   │   └── POST   /api/users/login     → Login (validación credenciales)
│   └── PostController.cs               → Endpoints REST para posts     (api/posts)
│       ├── GET    /api/posts           → GetAllPosts (paginado)
│       ├── GET    /api/posts/{id}      → GetPostById
│       ├── GET    /api/posts/user/{userId} → GetPostsByUserId
│       ├── POST   /api/posts           → CreatePost
│       ├── PUT    /api/posts/{id}      → UpdatePost
│       ├── DELETE /api/posts/{id}      → DeletePost
│       └── POST   /api/posts/{id}/media → UploadPostMedia (multipart/form-data)
│
├── Domain/
│   ├── Model/
│   │   ├── User.cs                     → Entidad de usuario (int UserId autoincremental)
│   │   └── Post.cs                     → Entidad de post (int PostId autoincremental)
│   ├── ValueObject/
│   │   ├── Tag.cs                      → Objeto de valor Tag (int tagId)
│   │   ├── UserPost.cs                 → Tabla intermedia User ↔ Post (relación N:M)
│   │   └── PostTag.cs                  → Tabla intermedia Post ↔ Tag (relación N:M)
│   └── Repository/
│       ├── IUserRepository.cs          → Puerto: interfaz de persistencia de usuarios
│       └── IPostRepository.cs          → Puerto: interfaz de persistencia de posts
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
│       ├── GetPostsByUserId/GetPostsByUserIdQueryHandler.cs
│       └── PagedResult.cs              → DTO para respuestas paginadas
│
└── Infrastructure/
    ├── IndieQuestDbContext.cs           → DbContext EF Core (mapeo entidades → tablas PostgreSQL)
    └── Repository/
        └── PostgreSQL/
            ├── PostgreSqlUserRepository.cs  → Adaptador: CRUD usuarios vía EF Core
            └── PostgreSqlPostRepository.cs  → Adaptador: CRUD posts vía EF Core
```

### Características principales

- **Upload de archivos multimedia** (500MB máx):
  - Fotos de perfil → `IndieQuest-LocalData/user/{id}/`
  - Contenido de posts → `IndieQuest-LocalData/postdata/{id}/`
  
- **Almacenamiento estático**: Configurado en `Program.cs` para servir `/IndieQuest-LocalData` como recurso público

- **CORS habilitado**: Permite solicitudes desde cualquier origen (`AllowAll` policy)

- **Swagger disponible** en `/swagger` (desarrollo)

- **Autenticación básica**: Endpoint de login que valida credenciales y devuelve datos del usuario

### Modelos de dominio (PostgreSQL)

#### `User`
| Propiedad              | Tipo       | Descripción                          |
|------------------------|------------|--------------------------------------|
| `UserId`               | `int`      | Autoincremental (SERIAL)             |
| `Username`             | `string`   | Nombre de usuario (único)            |
| `Password`             | `string`   | Contraseña (sin encriptación actual) |
| `Email`                | `string`   | Correo electrónico (único)           |
| `UserProfilePicture`   | `string?`  | Ruta al archivo de foto de perfil    |
| `UserBio`              | `string?`  | Biografía del usuario                |
| `AvailableForWork`     | `bool?`    | Disponibilidad para colaboraciones   |
| `dateOfRegistration`   | `DateTime` | Fecha UTC de alta                    |
| `UserPosts`            | `ICollection<UserPost>` | Navegación: posts del usuario |

#### `Post`
| Propiedad        | Tipo       | Descripción                          |
|------------------|------------|--------------------------------------|
| `PostId`         | `int`      | Autoincremental (SERIAL)             |
| `PostUserId`     | `int`      | FK a `User.UserId`                   |
| `Title`          | `string`   | Título del post                      |
| `MediaContent`   | `string?`  | Ruta al archivo multimedia           |
| `Description`    | `string?`  | Descripción/contenido del post       |
| `CreationDate`   | `DateTime` | Fecha UTC de creación                |
| `Tags`           | `ICollection<PostTag>` | Navegación: etiquetas |
| `UserPost`       | `UserPost` | Navegación inversa: autor           |

#### `Tag` (Value Object)
| Propiedad   | Tipo     | Descripción                       |
|-------------|----------|-----------------------------------|
| `tagId`     | `int`    | Autoincremental (SERIAL)          |
| `tagName`   | `string` | Nombre de la etiqueta (único)     |

#### `UserPost` & `PostTag` (Tablas intermedias)
Modelan relaciones N:M mediante PKs compuestas y FKs con cascade delete.

### Dependencias NuGet

| Paquete                                    | Versión  |
|--------------------------------------------|----------|
| `Swashbuckle.AspNetCore`                   | 10.1.7   |
| `Microsoft.EntityFrameworkCore`            | 10.0.0   |
| `Npgsql.EntityFrameworkCore.PostgreSQL`    | 10.0.0   |

### Configuración de base de datos

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=Indie_Quest_DB;User Id=postgres;Password=..."
  }
}
```

## 5. IndieQuest-Tests — Suite de Pruebas

Proyecto de pruebas que cubre la API en cuatro niveles de la pirámide de testing.

### Estructura de archivos

```
IndieQuest-Tests/
├── IndieQuest.Test.PosGre.csproj       → NUnit 4.2.2 + Moq 4.20.72 + MVC.Testing
│
├── UnitTest/                           → Pruebas de Query Handlers en aislamiento
├── IntegrationTest/                    → Pruebas de repositorios PostgreSQL
├── AcceptanceTest/                     → Pruebas de Command/Query Handlers con mocks
└── EndToEndTest/                       → Pruebas de Controllers vía HTTP
```

### Niveles de prueba

#### Unit Tests
Prueban Query Handlers inyectando mocks del repositorio. Validación de lógica pura sin dependencias externas.

#### Integration Tests
Prueban repositorios PostgreSQL directamente, verificando operaciones CRUD sin mocks.

#### Acceptance Tests
Prueban Command/Query Handlers con mocks, validando la lógica de orquestación de la capa de aplicación.

#### End-to-End Tests
Prueban controladores REST usando `WebApplicationFactory`, verificando respuestas HTTP reales.

### Dependencias NuGet

| Paquete                                   | Versión   |
|-------------------------------------------|-----------|
| `NUnit`                                   | 4.2.2     |
| `NUnit3TestAdapter`                       | 4.6.0     |
| `Microsoft.NET.Test.Sdk`                  | 17.12.0   |
| `Moq`                                     | 4.20.72   |
| `Microsoft.AspNetCore.Mvc.Testing`        | 10.0.0    |

## 6. IndieQuest-DataBase — Base de Datos

Scripts SQL para creación del esquema PostgreSQL y carga de datos iniciales.

### Estructura

```
IndieQuest-DataBase/
├── IQ-DB.sql                   → Creación de tablas, PKs, FKs, índices
└── IQ-StartingEntities.sql     → Datos de ejemplo (usuarios, posts, tags)
```

### Esquema

| Tabla       | Descrición                        | Relaciones                              |
|-------------|-----------------------------------|----------------------------------------|
| `"User"`    | Usuarios (3 ejemplos precargados) | 1:N → UserPost, 1:N → Post             |
| `Post`      | Posts (3 ejemplos precargados)    | N:1 → UserPost, N:M → Tag              |
| `Tag`       | Etiquetas (~3 ejemplos)           | N:M → PostTag                          |
| `Makes_MadeBy` | Relación User ↔ Post (N:M)   | FK a User + FK a Post (cascade delete) |
| `Has_Tag`   | Relación Post ↔ Tag (N:M)        | FK a Post + FK a Tag (cascade delete)  |

---

## 7. IndieQuest-UI — Frontend Principal

Aplicación React con Vite para usuarios finales. Interfaz responsive para gestionar perfil, ver posts y colaborar con otros desarrolladores.

### Estructura

```
IndieQuest-UI/
├── index.html                          → Punto de entrada HTML
├── package.json                        → Dependencias (React, React Router, Vite)
├── vite.config.js                      → Configuración Vite
├── README.md                           → Documentación del proyecto
├── public/                             → Recursos estáticos
├── src/
│   ├── main.jsx                        → Punto de entrada React
│   ├── App.jsx                         → Componente raíz
│   ├── api/
│   │   └── client.js                   → Cliente HTTP (llamadas a la API)
│   ├── context/
│   │   └── AuthContext.jsx             → Contexto de autenticación
│   ├── pages/
│   │   ├── HomePage.jsx
│   │   ├── LoginPage.jsx
│   │   ├── EditUserPage.jsx
│   │   └── ...otros
│   ├── components/
│   │   ├── PageHeader.jsx
│   │   ├── ErrorBox.jsx
│   │   ├── Spinner.jsx
│   │   └── ...otros
│   ├── styles/                         → Estilos CSS/SCSS
│   └── utils/
│       ├── format.js                   → Utilidades de formateo
│       └── ...otros
```

### Tecnologías

- **React 18**: Framework UI
- **React Router**: Navegación SPA
- **Vite**: Build tool (desarrollo rápido)
- **CSS/SCSS**: Estilos

---

## 8. IndieQuest-AdminInterface — Panel de Administración

Aplicación React con Vite para administradores. Interfaz para moderar contenido, gestionar usuarios y monitorear la plataforma.

### Estructura

```
IndieQuest-AdminInterface/
├── index.html
├── package.json                        → Dependencias React + Vite
├── vite.config.js                      → Configuración Vite
├── QUICK_START.md                      → Guía rápida de inicio
├── README.md                           → Documentación
├── ADMIN_PANEL_README.md               → Especificaciones del panel
├── ADMIN_TESTING_GUIDE.md              → Guía de testing
├── CHANGELOG.md                        → Historial de cambios
├── public/                             → Recursos estáticos
└── src/                                → Estructura similar a IndieQuest-UI
    ├── main.jsx
    ├── App.jsx
    ├── api/
    ├── context/
    ├── pages/
    ├── components/
    ├── styles/
    └── utils/
```

### Tecnologías

Mismas que IndieQuest-UI: **React 18**, **React Router**, **Vite**

---

## 9. IndieQuest-LocalData — Almacenamiento Multimedia

Carpeta de almacenamiento local para contenido multimedia (fotos de perfil y media de posts). **No se versionan archivos** en Git.

### Estructura

```
IndieQuest-LocalData/
├── .gitkeep                            → Marca la carpeta para Git (sin archivos)
├── user/
│   ├── 1/                              → Fotos de perfil del usuario 1
│   ├── 2/                              → Fotos de perfil del usuario 2
│   └── 3/                              → Fotos de perfil del usuario 3
└── postdata/
    ├── 1/                              → Media del post 1
    ├── 2/                              → Media del post 2
    └── 3/                              → Media del post 3
```

### Cómo funciona

1. **Upload**: Usuario sube archivo mediante `POST /api/users/{id}/picture` o `POST /api/posts/{id}/media`
2. **Almacenamiento**: API guarda el archivo en `IndieQuest-LocalData/{tipo}/{id}/`
3. **Ruta BD**: Se almacena la ruta relativa en PostgreSQL (ej: `IndieQuest-LocalData/user/1/avatar.jpg`)
4. **Servicio estático**: La API sirve estos archivos en `/IndieQuest-LocalData` como recurso público

### Límite de upload

Máximo **500 MB** por archivo (configurado en [Program.cs](IndieQuest-Api/Program.cs))

---

## 10. Diagrama de relaciones de datos

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

## Stack Tecnológico

| Capa                | Tecnología                                      |
|---------------------|-------------------------------------------------|
| **Backend API**     | ASP.NET Core 10.0 + EF Core 10.0 + Npgsql      |
| **Base de datos**   | PostgreSQL 16+                                  |
| **Frontend**        | React 18 + React Router + Vite                  |
| **Testing**         | NUnit 4.2.2 + Moq 4.20.72 + AspNetCore.Mvc.Testing |
| **Documentación**   | Swagger / Swashbuckle 10.1.7                    |
| **Arquitectura**    | Hexagonal (Ports & Adapters) + CQRS             |
| **API Style**       | REST (Richardson Level 2)                       |
| **Lenguaje**        | C# 13 / JavaScript (ES6+)                       |
