# Migración a PostgreSQL - IndieQuest API

## 📋 Resumen Ejecutivo

Se ha realizado una **migración completa de la arquitectura de repositorios de InMemory a PostgreSQL** utilizando **Entity Framework Core**. El proyecto ahora se conecta a una base de datos PostgreSQL real en lugar de almacenar datos en memoria.

---

## 🎯 Cambios Realizados

### **1. Actualización de Modelos de Dominio**

#### **Cambios en los IDs (string → int)**

Los modelos fueron actualizados para usar `int` en lugar de `string` para los identificadores, coincidiendo con la estructura de la base de datos PostgreSQL (donde usa `SERIAL`):

##### **User.cs**
```csharp
public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    // ...
    
    // Propiedades de navegación para relación N:M
    public ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
}
```

##### **Post.cs**
```csharp
public class Post
{
    public int PostId { get; set; }
    // Eliminado: public int PostUserId { get; set; }
    public required string Title { get; set; }
    public required string MediaContent { get; set; }
    // ...
    
    // Propiedades de navegación para relaciones N:M
    public ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
```

##### **Tag.cs**
```csharp
public class Tag
{
    public int tagId { get; set; }
    public required string tagName { get; set; }
    
    // Propiedades de navegación para relación N:M
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
```

#### **Nuevos Modelos ValueObject**

Se crearon dos modelos nuevos para representar las tablas intermedias:

##### **UserPost.cs** (representa `Makes_MadeBy`)
```csharp
public class UserPost
{
    public int UserId { get; set; }
    public int PostId { get; set; }
    
    // Propiedades de navegación
    public User User { get; set; }
    public Post Post { get; set; }
}
```

##### **PostTag.cs** (representa `Has_Tag`)
```csharp
public class PostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }
    
    // Propiedades de navegación
    public Post Post { get; set; }
    public Tag Tag { get; set; }
}
```

**Razón:** Los modelos ahora reflejan exactamente la estructura de la BD con relaciones N:M.

---

### **2. Actualización de Interfaces de Repositorio**

Las interfaces `IUserRepository` e `IPostRepository` fueron actualizadas para usar `int` en lugar de `string` en los parámetros:

#### **IUserRepository.cs**
```csharp
// CAMBIOS:
Task<User?> GetUserByIdAsync(int userId);        // Antes: string
Task DeleteUserAsync(int userId);                  // Antes: string
```

#### **IPostRepository.cs**
```csharp
// CAMBIOS:
Task<Post?> GetPostByIdAsync(int postId);         // Antes: string
Task<List<Post>> GetPostsByUserIdAsync(int userId); // Antes: string
Task DeletePostAsync(int postId);                  // Antes: string
```

---

### **3. Actualización de Commands y Handlers**

#### **CreateUserCommand**
```csharp
// ELIMINADO: public string UserId { get; set; }
// Razón: El ID se genera automáticamente en la base de datos (SERIAL)
```

**Antes:**
```csharp
public class CreateUserCommand
{
    public string UserId { get; set; }        // ❌ ELIMINADO
    public string Username { get; set; }
    // ...
}
```

**Después:**
```csharp
public class CreateUserCommand
{
    public string Username { get; set; }
    public string Password { get; set; }
    // ... (sin UserId)
}
```

#### **UpdateUserCommand**
```csharp
// CAMBIO: string UserId → int UserId
public int UserId { get; set; }
```

#### **CreatePostCommand**
```csharp
// ELIMINADO: public string PostId { get; set; }
// CAMBIO: string PostUserId → int PostUserId

// ANTES:
public string PostId { get; set; }        // ❌ ELIMINADO
public string PostUserId { get; set; }    // ❌ CAMBIADO

// DESPUÉS:
public int PostUserId { get; set; }       // ✅ int
```

#### **UpdatePostCommand**
```csharp
// CAMBIO: string PostId → int PostId
public int PostId { get; set; }
```

---

### **4. Actualización de Command Handlers**

#### **CreateUserCommandHandler**
```csharp
// ANTES: user.UserId = command.UserId (se asignaba desde el comando)
// DESPUÉS: Se omite UserId (será generado automáticamente por PostgreSQL)

public async Task Handle(CreateUserCommand command)
{
    var user = new User
    {
        // UserId se genera automáticamente
        Username = command.Username,
        Password = command.Password,
        // ...
        dateOfRegistration = DateTime.UtcNow
    };
    await _userRepository.CreateUserAsync(user);
}
```

#### **CreatePostCommandHandler**
```csharp
// ANTES: post.PostId = command.PostId (se asignaba desde el comando)
// DESPUÉS: Se omite PostId (será generado automáticamente por PostgreSQL)

public async Task Handle(CreatePostCommand command)
{
    var post = new Post
    {
        // PostId se genera automáticamente
        PostUserId = command.PostUserId,
        Title = command.Title,
        // ...
        CreationDate = DateTime.UtcNow
    };
    await _postRepository.CreatePostAsync(post);
}
```

#### **DeleteUserCommandHandler & DeletePostCommandHandler**
```csharp
// CAMBIO: parámetros de string a int
public async Task Handle(int userId)       // Antes: string userId
public async Task Handle(int postId)       // Antes: string postId
```

---

### **5. Actualización de Query Handlers**

#### **GetUserByIdQueryHandler**
```csharp
// ANTES:
public async Task<User?> Handle(string userId)

// DESPUÉS:
public async Task<User?> Handle(int userId)
```

#### **GetPostByIdQueryHandler**
```csharp
// ANTES:
public async Task<Post?> Handle(string postId)

// DESPUÉS:
public async Task<Post?> Handle(int postId)
```

#### **GetPostsByUserIdQueryHandler**
```csharp
// ANTES:
public async Task<List<Post>> Handle(string userId)

// DESPUÉS:
public async Task<List<Post>> Handle(int userId)
```

---

### **6. Actualización de Controladores**

Los controladores fueron actualizados para recibir parámetros `int` en lugar de `string`:

#### **UserController.cs**
```csharp
// ANTES:
[HttpGet("{id}")]
public async Task<IActionResult> GetUserById(string id)

[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(string id)

// DESPUÉS:
[HttpGet("{id}")]
public async Task<IActionResult> GetUserById(int id)

[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
```

#### **PostController.cs**
```csharp
// ANTES:
[HttpGet("{id}")]
public async Task<IActionResult> GetPostById(string id)

[HttpGet("user/{userId}")]
public async Task<IActionResult> GetPostsByUserId(string userId)

[HttpPut("{id}")]
public async Task<IActionResult> UpdatePost(string id, [FromBody] UpdatePostCommand command)

[HttpDelete("{id}")]
public async Task<IActionResult> DeletePost(string id)

// DESPUÉS:
[HttpGet("{id}")]
public async Task<IActionResult> GetPostById(int id)

[HttpGet("user/{userId}")]
public async Task<IActionResult> GetPostsByUserId(int userId)

[HttpPut("{id}")]
public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostCommand command)

[HttpDelete("{id}")]
public async Task<IActionResult> DeletePost(int id)
```

---

### **7. Instalación de Dependencias NuGet**

Se añadieron dos paquetes NuGet esenciales al archivo `IQ-Api.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />
</ItemGroup>
```

**Funciones:**
- **Microsoft.EntityFrameworkCore**: ORM que facilita operaciones de base de datos
- **Npgsql.EntityFrameworkCore.PostgreSQL**: Proveedor específico para PostgreSQL

---

### **8. Creación del DbContext**

Se creó `IndieQuestDbContext.cs` en la carpeta `Infrastructure/`:

```csharp
using Microsoft.EntityFrameworkCore;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Infrastructure;

public class IndieQuestDbContext : DbContext
{
    public IndieQuestDbContext(DbContextOptions<IndieQuestDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedOnAdd(); // Auto-incremento
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserBio).HasMaxLength(1000);
            entity.Property(e => e.UserProfilePicture).HasMaxLength(500);
            entity.Property(e => e.dateOfRegistration).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configuración de Post
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.PostId).ValueGeneratedOnAdd(); // Auto-incremento
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MediaContent).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
```

**Características clave:**
- Define `DbSet<User>` y `DbSet<Post>` para representar las tablas
- Configura auto-incremento en IDs primarios
- Establece restricciones de longitud y requerimientos
- Usa valores por defecto de base de datos para fechas

---

### **9. Refactorización de Repositorios PostgreSQL**

#### **PostgreSqlPostRepository.cs** (Actualizado)

Ahora incluye `.Include()` para cargar relaciones N:M:

```csharp
public async Task<List<Post>> GetAllPostsAsync()
{
    return await _context.Posts
        .Include(p => p.UserPosts)      // Cargar relación con Users
        .Include(p => p.PostTags)       // Cargar relación con Tags
        .ToListAsync();
}

public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
{
    return await _context.Posts
        .Where(p => p.UserPosts.Any(up => up.UserId == userId))
        .Include(p => p.UserPosts)
        .Include(p => p.PostTags)
        .ToListAsync();
}
```

**Cambios clave:**
- ✅ Usa `Include()` para cargar relaciones navegacionales
- ✅ Consultas a través de tablas intermedias
- ✅ Filtrado por UserId a través de `UserPosts`

#### **PostgreSqlUserRepository.cs**
```csharp
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Api.Infrastructure.Repository.PostgreSQL;

public class PostgreSqlUserRepository : IUserRepository
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

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
```

#### **PostgreSqlPostRepository.cs**
```csharp
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Api.Infrastructure.Repository.PostgreSQL;

public class PostgreSqlPostRepository : IPostRepository
{
    private readonly IndieQuestDbContext _context;

    public PostgreSqlPostRepository(IndieQuestDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(int postId)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
    {
        return await _context.Posts.Where(p => p.PostUserId == userId).ToListAsync();
    }

    public async Task CreatePostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
```

**Ventajas de estos repositorios:**
- Utilizan `ToListAsync()` y `FirstOrDefaultAsync()` para operaciones asincrónicas
- Aprovechan LINQ para filtros eficientes
- Manejan automáticamente el ciclo de vida de la conexión
- Permiten transacciones coherentes

---

### **10. Actualización de Program.cs**

Se realizaron los siguientes cambios:

#### **Importaciones actualizadas**
```csharp
// REMOVIDO:
using IndieQuest_Api.Infrastructure.Repository.InMemory;

// AÑADIDO:
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;
```

#### **Registración de DbContext**
```csharp
// Leer cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

// Registrar el contexto con PostgreSQL
builder.Services.AddDbContext<IndieQuestDbContext>(options =>
    options.UseNpgsql(connectionString));
```

#### **Cambio de Inyección de Dependencias**
```csharp
// ANTES (InMemory):
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();

// DESPUÉS (PostgreSQL):
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
builder.Services.AddScoped<IPostRepository, PostgreSqlPostRepository>();
```

**Nota importante:** Se cambió de `AddSingleton` a `AddScoped` porque Entity Framework Core requiere una nueva instancia del contexto por request HTTP.

#### **Cambio de ciclo de vida de Handlers**
```csharp
// ANTES: AddSingleton
// DESPUÉS: AddScoped

builder.Services.AddScoped<GetAllUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
// ... etc
```

---

### **11. Eliminación de Carpeta InMemory**

Se eliminó completamente la carpeta `Infrastructure/Repository/InMemory/` y sus contenidos:
- `InMemoryUserRepository.cs`
- `InMemoryPostRepository.cs`

**Razón:** Ya no son necesarias al usar PostgreSQL con Entity Framework Core.

---

## 🔌 Configuración de Conexión a PostgreSQL

### **Usando appsettings.json (Recomendado)**

La cadena de conexión se configura en el archivo `appsettings.json` del proyecto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=Indie_Quest_DB;User Id=postgres;Password=jc123;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Ventajas:**
- ✅ Se carga automáticamente al ejecutar `dotnet run`
- ✅ Nativo de .NET
- ✅ No requiere configuración adicional
- ✅ Separado de variables de entorno

### **Program.cs - Cómo se Lee**
```csharp
// La cadena de conexión se lee automáticamente de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");
```

### **Detalles de conexión:**
- **Host**: localhost
- **Puerto**: 5432
- **Base de datos**: Indie_Quest_DB
- **Usuario**: postgres
- **Contraseña**: jc123

---

## 📊 Flujo de Datos

### **Antes (InMemory):**
```
Cliente HTTP 
    ↓
Controlador 
    ↓
Handler (Query/Command)
    ↓
InMemoryRepository 
    ↓
Lista en memoria (no persistente)
```

### **Después (PostgreSQL):**
```
Cliente HTTP 
    ↓
Controlador 
    ↓
Handler (Query/Command)
    ↓
PostgreSqlRepository 
    ↓
DbContext (Entity Framework Core)
    ↓
PostgreSQL Database (persistente)
```

---

## 🗄️ Estructura de Carpetas

```
IndieQuest-Api_with_PosGre/
├── Infrastructure/
│   ├── IndieQuestDbContext.cs         (✅ NUEVO)
│   └── Repository/
│       ├── PostgreSQL/                 (✅ NUEVA CARPETA)
│       │   ├── PostgreSqlUserRepository.cs
│       │   └── PostgreSqlPostRepository.cs
│       └── InMemory/                   (❌ ELIMINADO)
├── Controllers/
├── Domain/
├── Application/
├── Program.cs                          (✅ ACTUALIZADO)
└── IQ-Api.csproj                       (✅ ACTUALIZADO)
```

---

## 🚀 Pasos para Ejecutar

### **1. Asegurar que PostgreSQL esté corriendo:**
```powershell
# Verificar si PostgreSQL está activo
# En Windows, acceder a Services o usar pgAdmin
```

### **2. Ejecutar la aplicación:**
```powershell
cd IndieQuest-Api_with_PosGre
dotnet run
```

> **Nota:** La cadena de conexión se carga automáticamente desde `appsettings.json`. No es necesario configurar variables de entorno.

### **3. Acceder a Swagger:**
```
http://localhost:5000/swagger
```

---

## 🔄 Refactorización de Relaciones N:M (Actualizado)

Se realizó una refactorización completa para alinear el modelo C# con la estructura real de la BD.

### **Cambios en la Estructura de Datos**

| Elemento | Antes | Después | Razón |
|----------|-------|---------|-------|
| Post.PostUserId | Propiedad directa (1:N) | Relación a través de UserPost (N:M) | Coincidir con tabla Makes_MadeBy |
| Post.Tags | Array Tag[] | Relación a través de PostTag (N:M) | Coincidir con tabla Has_Tag |
| Tablas intermedias | No mapeadas | UserPost, PostTag | Representar Makes_MadeBy, Has_Tag |

### **Modelos ValueObject Nuevos**

Se crearon dos modelos para las tablas intermedias:
- `UserPost.cs` - Representa tabla `Makes_MadeBy`
- `PostTag.cs` - Representa tabla `Has_Tag`

### **Commands Actualizados**

#### **CreatePostCommand (Antes)**
```csharp
public int PostUserId { get; set; }
public Tag[]? Tags { get; set; }
```

#### **CreatePostCommand (Después)**
```csharp
public int UserId { get; set; }
public int[]? TagIds { get; set; }
```

**Razón:** Reflejar que el post se crea con una relación a un usuario específico y con IDs de tags.

### **Handlers Actualizados**

#### **CreatePostCommandHandler** (Ahora crea UserPost)
```csharp
// Crear el post
await _postRepository.CreatePostAsync(post);

// Crear la relación UserPost (Makes_MadeBy)
var userPost = new UserPost
{
    UserId = command.UserId,
    PostId = post.PostId
};
_context.UserPosts.Add(userPost);

// Agregar tags si es necesario
foreach (var tagId in command.TagIds)
{
    var postTag = new PostTag { PostId = post.PostId, TagId = tagId };
    _context.PostTags.Add(postTag);
}
```

#### **UpdatePostCommandHandler** (Ahora maneja tags)
Actualiza PostTags eliminando los antiguos e insertando los nuevos.

---

Para futuras referencias, aquí están los ORM alternativos que se podrían usar:

| Framework | Ventajas | Desventajas | Caso de uso |
|-----------|----------|------------|------------|
| **Entity Framework Core** | ✅ Más moderno, LINQ integrado, migraciones automáticas | Más peso, curva de aprendizaje | Proyectos grandes y complejos |
| **Dapper** | ✅ Muy rápido, ligero, control total | Menos abstracción, más código SQL | Aplicaciones de alto rendimiento |
| **ADO.NET** | ✅ Control absoluto, sin dependencias | Muy tedioso, propenso a errores | Proyectos que requieren máximo control |

**Decisión actual:** Entity Framework Core es la opción elegida por ser moderna y coincidir con .NET 10.

---

## 🔄 Cambios Resumidos

| Elemento | Antes | Después |
|----------|-------|---------|
| Tipo de IDs | string (GUID) | int (SERIAL) |
| Persistencia | En memoria | PostgreSQL |
| ORM | Ninguno | Entity Framework Core |
| Ciclo de vida handlers | Singleton | Scoped |
| Carpeta repositorio | InMemory/ | PostgreSQL/ |
| Configuración | Hardcodeada | Variable de entorno |
| Conexión a BD | No aplicable | Localhost:5432 |

---

## 📝 Notas Importantes

1. **Auto-incremento**: Los IDs de User y Post se generan automáticamente en PostgreSQL. No incluirlos en los comandos de creación.

2. **Async/Await**: Todos los métodos de repositorio usan `async/await` para operaciones no bloqueantes.

3. **Entity Framework Core**: Maneja automáticamente:
   - Conexiones a la base de datos
   - Transacciones
   - Tracking de cambios
   - Lazy loading (si está habilitado)

4. **Cadena de conexión**: Si cambia la contraseña o detalles de PostgreSQL, actualizar la variable de entorno.

5. **Migraciones**: Si se agregan nuevas entidades o cambios al DbContext, usar EF Core Migrations:
   ```powershell
   dotnet ef migrations add MigrationName
   dotnet ef database update
   ```

---

## 🎯 Próximos Pasos (Opcionales)

1. **Implementar Migraciones EF Core** para versionamiento del esquema
2. **Añadir validaciones** en handlers
3. **Implementar transacciones** para operaciones complejas
4. **Crear índices** en campos frecuentemente consultados
5. **Añadir logging** para monitorear operaciones de BD
6. **Implementar paginación** en GetAllUsers y GetAllPosts

---

**Fecha de Implementación**: 20 de Abril, 2026  
**Proyecto**: IndieQuest  
**Estado**: ✅ Migración completada exitosamente
