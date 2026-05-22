# INDIE QUEST - TFG 2ºDAM 2026
#### AUTHOR: JUAN CARLOS PINAR FERREIRA

## SOBRE EL REPOSITORIO

Esté es un repositorio para documentar y gestiónar todo el desarollo de la aplicación INDIE QUEST, mi proyecto de fin de grado de Desarrollo de Aplicaciones Multiplataforma, del año 2026.

En esté repositorio, estaré gestionando tanto  el proyecto, como todas las pruebas y experimentos realizados para familiarizarme con las herramientas utilizadas. Se estará organizando el repo en dos carpetas principales: **INDIE-QUEST** y **PRUEBA**

A seguir, la organización de las mismas:


## INDIE-QUEST

### Resumen del proyecto
INDIE-QUEST es la API y backend del proyecto final. La solución principal contiene la API (`IndieQuest-Api`) y el proyecto de pruebas (`IndieQuest-Test`). El diseño sigue una arquitectura por capas (Application, Domain, Infrastructure, Controllers).

### Estructura principal (resumen)
- **IndieQuest-Api/**: API ASP.NET Core (proyecto `IQ-Api.csproj`).
  - `Program.cs`, `appsettings.json`, `appsettings.Development.json`
  - **Application/**: comandos y queries (CQRS)
  - **Controllers/**: controladores HTTP (ej. `PostController.cs`, `UserController.cs`)
  - **Domain/**: modelos, repositorios y value objects
  - **Infrastructure/**: implementaciones concretas de repositorios
  - **Properties/**: `launchSettings.json`

### Endpoints principales (resumen)
| Endpoint | Método | Descripción | Controller | Interactúa con |
|---|---:|---|---|---|
| `/api/posts` | GET | Obtener todos los posts | `PostController.cs` | `Domain.Model.Post`, repositorio de posts |
| `/api/posts/{id}` | GET | Obtener post por id | `PostController.cs` | repositorio de posts |
| `/api/posts/user/{userId}` | GET | Obtener posts por usuario | `PostController.cs` | repositorio de posts, `Domain.Model.User` |
| `/api/posts` | POST | Crear nuevo post | `PostController.cs` | `Application` command, repositorio de posts |
| `/api/posts/{id}` | PUT | Actualizar post | `PostController.cs` | repositorio de posts |
| `/api/posts/{id}` | DELETE | Eliminar post | `PostController.cs` | repositorio de posts |
| `/api/users` | GET | Obtener todos los usuarios | `UserController.cs` | `Domain.Model.User`, repositorio de usuarios |
| `/api/users/{id}` | GET | Obtener usuario por id | `UserController.cs` | repositorio de usuarios |
| `/api/users` | POST | Crear usuario | `UserController.cs` | `Application` command, repositorio de usuarios |
| `/api/users/{id}` | PUT | Actualizar usuario | `UserController.cs` | repositorio de usuarios |
| `/api/users/{id}` | DELETE | Eliminar usuario | `UserController.cs` | repositorio de usuarios |



### Clases y responsabilidades destacadas
- `PostController.cs`: expone las operaciones CRUD para posts y orquesta `Application` handlers.
- `UserController.cs`: expone operaciones CRUD para usuarios.
- `Application/Command` y `Application/Queries`: handlers que implementan la lógica de casos de uso (create, update, delete, get).
- `Infrastructure/Repository`: implementaciones concretas que cumplen los contratos de repositorio definidos en `Domain/Repository`.

### Tests
En el repositorio hay pruebas organizadas en `IndieQuest-Test/` con las siguientes carpetas (no se documentan aquí individualmente):
- `AcceptanceTest`
- `EndToEndTest`
- `IntegrationTest`
- `UnitTest`

---

### IndieQuest-UI
Frontend principal de INDIE QUEST. Interfaz de usuario para la plataforma social de desarrolladores de videojuegos independientes.
- **Stack tecnológico**: React + Vite
- **Características principales**: 
  - Interfaz responsiva
  - Navegación con React Router
  - Consumo de API REST
  - Gestión de usuarios y publicaciones
- **EJECUCIÓN:**
  - Instalar dependencias: `npm install`
  - Ejecutar en desarrollo: `npm run dev`
  - La API debe estar disponible en `http://localhost:5063` (configurable via `.env`)

---

### IndieQuest-AdminInterface
Panel de administración para gestión de la plataforma INDIE QUEST.
- **Stack tecnológico**: React + Vite
- **Características principales**:
  - Gestión de usuarios (ver, editar, eliminar)
  - Gestión de publicaciones
  - Filtrado avanzado
  - Acceso administrativo completo
  - Badge de "ADMIN MODE"
- **EJECUCIÓN:**
  - Instalar dependencias: `npm install`
  - Configurar API (opcional): crear `.env.local` con `VITE_API_URL=http://your-api-server:port/api`
  - Ejecutar en desarrollo: `npm run dev`
  - Acceder en: `http://localhost:5173`
- **Notas**: No requiere login - acceso directo como "AdminPanel"

---

### IndieQuest-DataBase
Scripts SQL para configuración y gestión de la base de datos PostgreSQL.
- **Archivos principales**:
  - `IQ-DB.sql`: Script de creación y reset de la base de datos
  - `IQ-StartingEntities.sql`: Datos iniciales de prueba
- **Esquema**:
  - Tabla `User`: gestión de usuarios (id, nombre, email, foto perfil, bio, disponibilidad)
  - Tabla `Post`: publicaciones de contenido (id, título, media, descripción, fecha)
  - Tabla `Tag`: etiquetas para clasificar posts
  - Relación `Makes_MadeBy` (1:N): usuario crea post
  - Relación `Has_Tag` (N:M): post tiene múltiples tags
- **Base de datos**: PostgreSQL
- **EJECUCIÓN**: Ejecutar scripts en PostgreSQL, preferentemente a través de Docker Compose

---

### IndieQuest-LocalData
Almacenamiento local de archivos multimedia (imágenes de perfil y contenido de posts).
- **Estructura**:
  - `user/{id}/profile.jpg`: Fotos de perfil de usuarios
  - `postdata/{id}/media.jpg`: Contenido multimedia de posts
- **Acceso vía API**:
  - `http://localhost:5000/IndieQuest-LocalData/user/{id}/profile.jpg`
  - `http://localhost:5000/IndieQuest-LocalData/postdata/{id}/media.jpg`


---

## PRUEBA
* **Markdowns_PlanesDeApoyo**
    * Colección de guías y documentación de apoyo que han sido utilizados para el proyecto INDIE QUEST
   

* **OpenLibrary**
    * Proyecto inicial realizado durante las prácticas, sirviendo de introducción a C#, APIs en .NET y REACT, junto a la arquitectura Hexagonal. También se ha aplicado base64 para gestión de imagenes.
    * En este proyecto, también se ha trabajado el desarollo de tests de Aceptación, End to End y Unitarios.
    * **EJECUCIÓN:**
        * 1- Inicializar la API. Abrir proyecto open library y ejecutar **donet run**
        * 2- Inicializar la interfaz grafica, ejecutar **npm run dev** dentro del proyecto.
    * **IMPORTANTE:** recordar ejecutar el comando **npm ci** dentro de la interfaz para instalar los paquetes necesarios de REACT. Puede ser posible que también se tenga que cambiar la url de los endpoints en las llamadas a la API.

* **PruebaDatosLocales-IndieQuest**
    * Proyectos locales de prueba para validar la funcionalidad de almacenamiento de datos en el contexto de INDIE QUEST
    * Estructura:
        - `IndieQuest-ApiLocal/`: API local para pruebas
        - `IndieQuest-TestLocal/`: Suite de pruebas local

* **Testing1** 
    * Pequeña práctica de Implementación de SWAGGER y desarollo de API en .net básica.
