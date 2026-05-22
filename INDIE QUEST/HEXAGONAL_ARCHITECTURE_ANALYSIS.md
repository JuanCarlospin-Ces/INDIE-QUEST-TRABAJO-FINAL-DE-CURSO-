# Arquitectura Hexagonal en IndieQuest-Api

## ¿Qué es la Arquitectura Hexagonal?

La **Arquitectura Hexagonal** (también llamada *Ports & Adapters*, propuesta por Alistair Cockburn) es un patrón arquitectónico que organiza el código en capas concéntricas donde:

- **El núcleo** (dominio) está completamente aislado del mundo exterior
- **Las capas interiores NO conocen a las capas exteriores**
- La comunicación entre capas se realiza a través de **puertos** (interfaces)
- Las implementaciones concretas son **adaptadores** que se conectan desde fuera

### Objetivo principal

Permitir que la lógica de negocio sea **independiente de los detalles técnicos** (base de datos, frameworks HTTP, etc.). Esto facilita:
- Testing sin dependencias externas
- Cambio de tecnología sin modificar el dominio
- Escalabilidad y mantenibilidad

---

## Estructura de la Arquitectura Hexagonal

```
┌─────────────────────────────────────────────────────────┐
│         CAPA DE PRESENTACIÓN (External)                 │
│  Controllers (HTTP Adapters)                            │
│  • UserController                                        │
│  • PostController                                        │
└──────────────────┬──────────────────────────────────────┘
                   │ invoca handlers via interfaces
┌──────────────────▼──────────────────────────────────────┐
│         CAPA DE APLICACIÓN (Application)                │
│  Command/Query Handlers - Orquestación                  │
│  • CreateUserCommandHandler                             │
│  • GetAllUsersQueryHandler                              │
│  ↓ dependen de puertos (IUserRepository, ...)           │
└──────────────────┬──────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │ (Puertos/Interfaces)│
        └──────────┬──────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│         CAPA DE DOMINIO (NÚCLEO - Internal)             │
│  ✓ Modelos: User, Post, Tag, UserPost, PostTag         │
│  ✓ Puertos: IUserRepository, IPostRepository            │
│  ✓ ValueObjects                                          │
│  ✓ SIN dependencias externas                            │
│  ✓ SIN acceso a BD, HTTP, filesystems                   │
└──────────────────────────────────────────────────────────┘
                   │ implementado por
        ┌──────────▼──────────┐
        │ (Adaptadores)       │
        └──────────┬──────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│    CAPA DE INFRAESTRUCTURA (External Adapters)          │
│  Repositorios concretos - BD, I/O                       │
│  • PostgreSqlUserRepository (EF Core + Npgsql)          │
│  • PostgreSqlPostRepository (EF Core + Npgsql)          │
│  • IndieQuestDbContext (ORM)                            │
└──────────────────────────────────────────────────────────┘
```

---

## ✅ Verificación: IndieQuest-Api SÍ Implementa Arquitectura Hexagonal

La API implementa correctamente los componentes de la arquitectura hexagonal:

### 1. **PUERTOS (Interfaces en el Dominio)**

Los puertos son las interfaces que **define el dominio** y que **implementan los adaptadores**. Están en `Domain/Repository/`:

#### `Domain/Repository/IUserRepository.cs`
```csharp
using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<(List<User> Items, int TotalCount)> GetAllUsersPagedAsync(int pageNumber, int pageSize, bool? availableForWork = null);
    Task<User?> GetUserByIdAsync(int userId);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}
```

**Ubicación**: `IndieQuest-Api/Domain/Repository/IUserRepository.cs`

#### `Domain/Repository/IPostRepository.cs`
Similar a `IUserRepository`, define el contrato para operaciones CRUD de posts.

**Ubicación**: `IndieQuest-Api/Domain/Repository/IPostRepository.cs`

**¿Por qué es un puerto?**
- Es una interfaz definida en el dominio
- El dominio NO sabe qué tecnología la implementa (puede ser BD relacional, NoSQL, archivos, etc.)
- Es la frontera entre el dominio y el mundo exterior

---

### 2. **MODELOS DE DOMINIO**

El dominio contiene modelos puros sin dependencias externas.

#### `Domain/Model/User.cs`
```csharp
using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Domain.Model;

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool? AvailableForWork { get; set; }
    public string? UserBio { get; set; }
    public string? UserProfilePicture { get; set; }
    public required string Email { get; set; }
    public DateTime dateOfRegistration { get; set; }
    
    // Propiedades de navegación
    public ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
}
```

**Ubicación**: `IndieQuest-Api/Domain/Model/User.cs`

**Características**:
- ✅ Define la estructura de datos
- ✅ Sin lógica de persistencia
- ✅ Sin referencias a EF Core, Npgsql, HTTP
- ✅ Solo contiene lógica de negocio pura

#### `Domain/Model/Post.cs`
Similar a `User`, modela las publicaciones sin detalles técnicos.

**Ubicación**: `IndieQuest-Api/Domain/Model/Post.cs`

#### `Domain/ValueObject/Tag.cs`, `UserPost.cs`, `PostTag.cs`
Value Objects que modelan conceptos del dominio.

**Ubicación**: `IndieQuest-Api/Domain/ValueObject/`

---

### 3. **CAPA DE APLICACIÓN (Handlers)**

Los handlers orquestan el flujo entre el dominio y los adaptadores. Reciben puertos (interfaces) por inyección de dependencias.

#### `Application/Command/Users/CreateUserCommandHandler.cs`
```csharp
using System;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Users;

public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;  // ← PUERTO (interfaz)

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(CreateUserCommand command)
    {
        var user = new User
        {
            Username = command.Username,
            Password = command.Password,
            AvailableForWork = command.AvailableForWork,
            UserBio = command.UserBio,
            UserProfilePicture = null,
            Email = command.Email,
            dateOfRegistration = DateTime.UtcNow
        };

        await _userRepository.CreateUserAsync(user);  // ← Usa el puerto

        var userFolder = $"IndieQuest-LocalData/user/{user.UserId}";
        Directory.CreateDirectory(userFolder);

        if (!string.IsNullOrEmpty(command.UserProfilePicture))
        {
            user.UserProfilePicture = $"{userFolder}/{command.UserProfilePicture}";
            await _userRepository.UpdateUserAsync(user);
        }

        return user;
    }
}
```

**Ubicación**: `IndieQuest-Api/Application/Command/Users/CreateUserCommandHandler.cs`

**¿Por qué es arquitectura hexagonal?**
- ✅ El handler depende de `IUserRepository` (interfaz/puerto)
- ✅ NO sabe si la implementación es PostgreSQL, MongoDB, archivos, etc.
- ✅ Puede testearse fácilmente con un mock del repositorio
- ✅ Separación clara entre lógica de aplicación y persistencia

#### Otros Handlers
```
Application/
├── Command/
│   ├── Users/
│   │   ├── CreateUserCommandHandler.cs    ← Depende de IUserRepository
│   │   ├── UpdateUserCommandHandler.cs
│   │   └── DeleteUserCommandHandler.cs
│   └── Posts/
│       ├── CreatePostCommandHandler.cs    ← Depende de IPostRepository
│       ├── UpdatePostCommandHandler.cs
│       └── DeletePostCommandHandler.cs
└── Queries/
    ├── GetAllUsers/GetAllUsersQueryHandler.cs
    ├── GetUserById/GetUserByIdQueryHandler.cs
    ├── GetAllPosts/GetAllPostsQueryHandler.cs
    ├── GetPostById/GetPostByIdQueryHandler.cs
    └── GetPostsByUserId/GetPostsByUserIdQueryHandler.cs
```

**Ubicación**: `IndieQuest-Api/Application/`

---

### 4. **ADAPTADORES (Implementaciones Concretas)**

Los adaptadores son la realización concreta de los puertos. En este proyecto, todos usan PostgreSQL + EF Core.

#### `Infrastructure/Repository/PostgreSQL/PostgreSqlUserRepository.cs`
```csharp
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace IndieQuest_Api.Infrastructure.Repository.PostgreSQL;

public class PostgreSqlUserRepository : IUserRepository  // ← IMPLEMENTA el puerto
{
    private readonly IndieQuestDbContext _context;

    public PostgreSqlUserRepository(IndieQuestDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<(List<User> Items, int TotalCount)> GetAllUsersPagedAsync(
        int pageNumber, int pageSize, bool? availableForWork = null)
    {
        IQueryable<User> query = _context.Users;

        if (availableForWork.HasValue)
        {
            query = query.Where(u => u.AvailableForWork == availableForWork.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.UserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }
    
    // ... otros métodos CRUD
}
```

**Ubicación**: `IndieQuest-Api/Infrastructure/Repository/PostgreSQL/PostgreSqlUserRepository.cs`

**Características**:
- ✅ Implementa la interfaz `IUserRepository`
- ✅ Contiene detalles técnicos de BD (EF Core, LINQ, Npgsql)
- ✅ El dominio NO sabe esto existe
- ✅ Puede ser reemplazado por otra implementación (ej: MongoDB) sin cambiar handlers

#### `Infrastructure/Repository/PostgreSQL/PostgreSqlPostRepository.cs`
Similar a `PostgreSqlUserRepository`, implementa `IPostRepository`.

**Ubicación**: `IndieQuest-Api/Infrastructure/Repository/PostgreSQL/PostgreSqlPostRepository.cs`

#### `Infrastructure/IndieQuestDbContext.cs`
El DbContext de EF Core que mapea modelos de dominio a tablas PostgreSQL.

**Ubicación**: `IndieQuest-Api/Infrastructure/IndieQuestDbContext.cs`

---

### 5. **CONTROLADORES (HTTP Adapters)**

Los controladores son adaptadores que convierten peticiones HTTP en llamadas a handlers.

#### `Controllers/UserController.cs`
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
{
    var user = await _createUserCommandHandler.Handle(command);
    return Ok(new { userId = user.UserId });
}
```

**Ubicación**: `IndieQuest-Api/Controllers/UserController.cs`

#### `Controllers/PostController.cs`
```csharp
[HttpPost("{id}/media")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UploadPostMedia(int id, IFormFile file)
{
    // ... manejo de archivo
}
```

**Ubicación**: `IndieQuest-Api/Controllers/PostController.cs`

---

### 6. **INYECCIÓN DE DEPENDENCIAS**

En `Program.cs` se registra qué **adaptador concreto** implementa cada puerto:

```csharp
// Register repositories with PostgreSQL implementations
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
builder.Services.AddScoped<IPostRepository, PostgreSqlPostRepository>();
```

**Ubicación**: `IndieQuest-Api/Program.cs` (líneas ~60)

**¿Por qué es crucial para hexagonal?**
- ✅ Los handlers NO instancian adaptadores directamente
- ✅ Reciben el puerto (interfaz) inyectado
- ✅ Para cambiar a otra BD, solo se modifica esta configuración

---

## Beneficios de la Arquitectura en IndieQuest-Api

### 1. **Independencia del Dominio**
```
ANTES (mala práctica):
CreateUserCommandHandler → PostgreSqlUserRepository (acoplado a BD)

DESPUÉS (hexagonal):
CreateUserCommandHandler → IUserRepository → PostgreSqlUserRepository
                           (abstracto)       (concreto)
```

### 2. **Testing Simplificado**
```csharp
// En tests: inyectar un mock en lugar de la BD real
var mockRepository = new Mock<IUserRepository>();
mockRepository.Setup(r => r.GetUserByIdAsync(1))
    .ReturnsAsync(new User { UserId = 1, Username = "test" });

var handler = new GetUserByIdQueryHandler(mockRepository.Object);
var result = await handler.Handle(1);
```

### 3. **Flexibilidad Tecnológica**
Cambiar de PostgreSQL a otra BD requiere solo:
- Crear nueva clase `MongoDbUserRepository : IUserRepository`
- Modificar una línea en `Program.cs`
- Sin tocar handlers, modelos, ni controladores

### 4. **Mantenibilidad**
```
Proyecto crece sin complejidad:
- Dominio: Focus en reglas de negocio
- Aplicación: Focus en orquestación
- Infraestructura: Focus en detalles técnicos
Cada capa es independiente y testeable
```

---

## Mapa de Archivos Clave

| Componente | Ubicación | Propósito |
|------------|-----------|----------|
| **PUERTOS** | `Domain/Repository/IUserRepository.cs` | Define contrato de persistencia |
| | `Domain/Repository/IPostRepository.cs` | Define contrato de persistencia |
| **MODELOS** | `Domain/Model/User.cs` | Entidad de usuario |
| | `Domain/Model/Post.cs` | Entidad de post |
| **VALUE OBJ** | `Domain/ValueObject/Tag.cs` | Objeto de valor |
| | `Domain/ValueObject/UserPost.cs` | Relación N:M |
| **HANDLERS** | `Application/Command/Users/CreateUserCommandHandler.cs` | Orquestación |
| | `Application/Queries/GetUserById/GetUserByIdQueryHandler.cs` | Consulta |
| **ADAPTADORES** | `Infrastructure/Repository/PostgreSQL/PostgreSqlUserRepository.cs` | Implementación BD |
| | `Infrastructure/Repository/PostgreSQL/PostgreSqlPostRepository.cs` | Implementación BD |
| | `Infrastructure/IndieQuestDbContext.cs` | ORM Mapping |
| **HTTP ADAPTERS** | `Controllers/UserController.cs` | API Endpoints |
| | `Controllers/PostController.cs` | API Endpoints |
| **DI CONFIG** | `Program.cs` (líneas ~60) | Registro de adaptadores |

---

## Conclusión

✅ **IndieQuest-Api implementa correctamente la Arquitectura Hexagonal** con:

1. ✓ **Dominio puro** (modelos sin dependencias externas)
2. ✓ **Puertos bien definidos** (interfaces `IUserRepository`, `IPostRepository`)
3. ✓ **Adaptadores concretos** (`PostgreSqlUserRepository`, `PostgreSqlPostRepository`)
4. ✓ **Capa de aplicación** que orquesta sin conocer detalles técnicos
5. ✓ **Inyección de dependencias** para desacoplamiento
6. ✓ **Controladores como adaptadores HTTP**

La arquitectura permite que la API sea:
- **Escalable**: Nuevas características sin refactoring
- **Testeable**: Mocks fáciles en tests unitarios
- **Flexible**: Cambio de BD sin tocar lógica
- **Mantenible**: Separación clara de responsabilidades
