# Sistema de Almacenamiento Local de Medios - IndieQuest

## 📋 Resumen Ejecutivo

Se ha implementado un sistema de almacenamiento **local** para:
- **Fotos de Perfil de Usuarios** → `IndieQuest-LocalData/user/{userId}/`
- **Contenido Multimedia de Posts** → `IndieQuest-LocalData/postdata/{postId}/`

---

## ✅ Viabilidad Confirmada

La implementación es **100% viable** por las siguientes razones:

### 1. Modelos Compatible
- **User.cs**: Propiedad `UserProfilePicture` (string)
- **Post.cs**: Propiedad `MediaContent` (string)
- Ambas propiedades pueden almacenar rutas locales sin modificación del modelo

### 2. Estructura Base de Datos
- **Tabla User**: `idUser` (SERIAL)
- **Tabla Post**: `idPost` (SERIAL)
- Las claves primarias numéricas permiten crear carpetas de forma correlativa

### 3. Infraestructura de Carpetas
- Existen carpetas base: `user/` y `postdata/`
- Windows permite crear subdirectorios con IDs numéricos

---

## 📁 Estructura Creada

### A. Carpetas de Usuarios
```
IndieQuest-LocalData/
└── user/
    ├── 1/          (john_doe)
    ├── 2/          (jane_smith)
    └── 3/          (alice_wonder)
```

**Ubicación**: `IndieQuest-LocalData/user/{userId}/`
- **Propósito**: Almacenar fotos de perfil
- **Usuarios existentes**: 3 (IDs: 1, 2, 3)

### B. Carpetas de Posts
```
IndieQuest-LocalData/
└── postdata/
    ├── 1/          (First Post)
    ├── 2/          (Second Post)
    └── 3/          (Third Post)
```

**Ubicación**: `IndieQuest-LocalData/postdata/{postId}/`
- **Propósito**: Almacenar contenido multimedia (imágenes, videos, etc.)
- **Posts existentes**: 3 (IDs: 1, 2, 3)

---

## 🔄 Flujo de Funcionamiento Propuesto

### 1. Subida de Foto de Perfil
```
Frontend (Form Upload)
    ↓
Backend (PostController/UserController)
    ↓
Guardar en: IndieQuest-LocalData/user/{userId}/archivo.jpg
    ↓
Actualizar: User.UserProfilePicture = "~/IndieQuest-LocalData/user/{userId}/archivo.jpg"
    ↓
Base de Datos (Actualizar campo ProfilePicture)
    ↓
Frontend (Acceso Local)
```

### 2. Subida de Contenido Multimedia de Post
```
Frontend (Form Upload)
    ↓
Backend (PostController)
    ↓
Guardar en: IndieQuest-LocalData/postdata/{postId}/archivo.jpg
    ↓
Actualizar: Post.MediaContent = "~/IndieQuest-LocalData/postdata/{postId}/archivo.jpg"
    ↓
Base de Datos (Actualizar campo mediaContent)
    ↓
Frontend (Acceso Local)
```

---

## 💾 Datos Existentes en Base de Datos

### Usuarios
| ID | Username | Email | Biografía |
|---|---|---|---|
| 1 | john_doe | john.doe@example.com | Software developer with a passion for open-source projects. |
| 2 | jane_smith | jane.smith@example.com | Graphic designer specializing in digital art and branding. |
| 3 | alice_wonder | alice.wonder@example.com | Content creator and social media manager with a love for storytelling. |

### Posts
| ID | Título | Descripción | Usuario |
|---|---|---|---|
| 1 | First Post | This is the first post. | john_doe (ID: 1) |
| 2 | Second Post | This is the second post. | jane_smith (ID: 2) |
| 3 | Third Post | This is the third post. | alice_wonder (ID: 3) |

---

## 📝 Próximos Pasos

### 1. Modificar Controladores (Backend)
**Archivos a actualizar:**
- `IndieQuest-Api/Controllers/UserController.cs`
- `IndieQuest-Api/Controllers/PostController.cs`

**Cambios necesarios:**
```csharp
// Ejemplo para subida de foto de perfil
string uploadPath = Path.Combine("IndieQuest-LocalData/user", userId.ToString());
Directory.CreateDirectory(uploadPath); // Crear carpeta si no existe
string filePath = Path.Combine(uploadPath, fileName);
// Guardar archivo...
// Actualizar: user.UserProfilePicture = filePath;
```

### 2. Configurar Acceso Estático (Program.cs)
Para que los archivos sean accesibles vía HTTP:
```csharp
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "IndieQuest-LocalData")),
    RequestPath = "/media"
});
```

Con esto, las imágenes serían accesibles en: `http://localhost:5000/media/user/1/foto.jpg`

### 3. Actualizar URLs en Base de Datos (Opcional)
Si lo deseas, puedes actualizar las URLs actuales:
```sql
-- Para usuarios
UPDATE "User" 
SET ProfilePicture = 'IndieQuest-LocalData/user/' || idUser || '/profile.jpg'
WHERE ProfilePicture IS NOT NULL;

-- Para posts
UPDATE Post 
SET mediaContent = 'IndieQuest-LocalData/postdata/' || idPost || '/media.jpg'
WHERE mediaContent IS NOT NULL;
```

### 4. Implementar Validaciones
- Tipos de archivo permitidos (jpg, png, gif, mp4, etc.)
- Tamaño máximo de archivo
- Manejo de errores en caso de escritura fallida

---

## 🛠️ Modelos Involucrados

### User.cs
```csharp
public class User
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool? AvailableForWork { get; set; }
    public string? UserBio { get; set; }
    public string? UserProfilePicture { get; set; }  // ← AQUÍ va la ruta local
    public string Email { get; set; }
    public DateTime dateOfRegistration { get; set; }
}
```

### Post.cs
```csharp
public class Post
{
    public string PostId { get; set; }
    public string PostUserId { get; set; }
    public string Title { get; set; }
    public string MediaContent { get; set; }  // ← AQUÍ va la ruta local
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }
    public Tag[]? Tags { get; set; }
}
```

---

## 🔐 Consideraciones de Seguridad

1. **Validar extensiones de archivo** antes de guardar
2. **Sanitizar nombres de archivo** para evitar inyecciones de ruta
3. **Limitar tamaño de archivo** (ej: máximo 5MB para imágenes)
4. **Verificar permisos** antes de permitir acceso a archivos
5. **Usar rutas relativas** en lugar de absolutas cuando sea posible

---

## 📊 Resumen de Cambios Realizados

| Elemento | Estado | Detalles |
|---|---|---|
| Carpetas de usuarios | ✅ Creadas | 3 carpetas (IDs: 1, 2, 3) |
| Carpetas de posts | ✅ Creadas | 3 carpetas (IDs: 1, 2, 3) |
| Carpeta Markdowns | ✅ Creada | Para documentación |
| Modelos | ✅ Compatible | No requieren cambios |
| Base de Datos | ✅ Compatible | Estructura existente soporta esta solución |
| Controladores | ⏳ Pendiente | Requieren implementación |
| Program.cs | ⏳ Pendiente | Requiere configuración de archivos estáticos |

---

## 📞 Notas Finales

- Las carpetas están **listas** para recibir archivos
- Puedes **ingresar manualmente las imágenes** en cada carpeta como indiques
- La implementación es **modular**: cada ID tiene su propia carpeta
- El sistema es **escalable**: nuevas usuarios/posts crearán nuevas carpetas automáticamente
- Esta solución es **compatible** con desarrollo local y producción

---

**Fecha de Creación**: 20 de Abril, 2026  
**Proyecto**: IndieQuest  
**Estado**: ✅ Estructura lista para implementación
