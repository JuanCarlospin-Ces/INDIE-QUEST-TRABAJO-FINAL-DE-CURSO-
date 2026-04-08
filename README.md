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


## PRUEBA
* **OpenLibrary**
    * Proyecto inicial realizado durante las prácticas, sirviendo de introducción a C#, APIs en .NET y REACT, junto a la arquitectura Hexagonal. También se ha aplicado base64 para gestión de imagenes.
    * En este proyecto, también se ha trabajado el desarollo de tests de Aceptación, End to End y Unitarios.
    * **EJECUCIÓN:**
        * 1- Inicializar la API. Abrir proyecto open library y ejecutar **donet run**
        * 2- Inicializar la interfaz grafica, ejecutar **npm run dev** dentro del proyecto.
    * **IMPORTANTE:** recordar ejecutar el comando **npm ci** dentro de la interfaz para instalar los paquetes necesarios de REACT. Puede ser posible que también se tenga que cambiar la url de los endpoints en las llamadas a la API.
* **Testing1** 
    * Pequeña práctica de Implementación de SWAGGER y desarollo de API en .net básica.
* **Testeando_MiProyecto**
    * Pruebas generelas a respecto de la aplicación. Un proyecto dedicado únicamente a probar cosas que puedan ser implementadas en el proyecto final o no.