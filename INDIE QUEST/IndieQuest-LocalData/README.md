# IndieQuest-LocalData

Carpeta raíz para almacenar archivos multimedia y datos locales del proyecto IndieQuest.

## Estructura

```
IndieQuest-LocalData/
├── user/
│   ├── 1/
│   │   └── profile.jpg (foto de perfil del usuario con ID 1)
│   ├── 2/
│   │   └── profile.jpg
│   └── ... (hasta usuario 15 y más)
│
└── postdata/
    ├── 1/
    │   └── media.jpg (contenido multimedia del post con ID 1)
    ├── 2/
    │   └── media.jpg
    └── ... (hasta post 25 y más)
```

## Cómo acceder a los archivos

### Desde la TestingInterface (Frontend)

La API sirve estos archivos en la siguiente ruta:

```
http://localhost:5000/IndieQuest-LocalData/user/{id}/profile.jpg
http://localhost:5000/IndieQuest-LocalData/postdata/{id}/media.jpg
```

**Ejemplo:**
- `http://localhost:5000/IndieQuest-LocalData/user/1/profile.jpg` → Foto de perfil del usuario 1
- `http://localhost:5000/IndieQuest-LocalData/postdata/5/media.jpg` → Contenido multimedia del post 5

### Desde la base de datos

En la base de datos, los campos `ProfilePicture` y `mediaContent` almacenan las rutas relativas:

```sql
-- Usuario
'IndieQuest-LocalData/user/1/profile.jpg'

-- Post
'IndieQuest-LocalData/postdata/1/media.jpg'
```

## Configuración en la API

El archivo [Program.cs](../IndieQuest-Api/Program.cs) está configurado para servir archivos estáticos:

```csharp
var staticFileOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "IndieQuest-LocalData")),
    RequestPath = "/IndieQuest-LocalData"
};
app.UseStaticFiles(staticFileOptions);
```

## Notas importantes

- ✅ Esta carpeta está **separada de la lógica de la aplicación** (a nivel de raíz)
- ✅ Se puede respaldar y gestionar independientemente
- ✅ Es compartible entre múltiples servicios
- ⚠️ Asegúrate de que la API esté corriendo para acceder a los archivos
- ⚠️ Los archivos deben subirse respetando la estructura: `user/{id}/` o `postdata/{id}/`
