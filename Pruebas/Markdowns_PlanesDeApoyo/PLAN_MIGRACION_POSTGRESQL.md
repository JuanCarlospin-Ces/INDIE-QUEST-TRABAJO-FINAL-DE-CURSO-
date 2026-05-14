# Plan de Migración a PostgreSQL para IndieQuest

## Resumen

Este plan detalla los pasos para migrar la API de IndieQuest de un almacenamiento InMemory a una base de datos PostgreSQL usando Entity Framework Core, asegurando alineación con el esquema SQL y buenas prácticas de arquitectura, seguridad y testeo.

---

## Fase 1 — Verificar alineación DbContext ↔ Esquema SQL
- Comparar `IndieQuestDbContext.OnModelCreating` con `IQ-DB.sql`.
- Confirmar que los nombres de columnas y tablas coinciden exactamente.
- Verificar claves primarias, foráneas y valores por defecto.

## Fase 2 — Crear y aplicar migraciones EF Core
- Instalar herramientas: `dotnet tool install --global dotnet-ef`.
- Crear migración: `dotnet ef migrations add InitialCreate`.
- Validar que el SQL generado coincide con `IQ-DB.sql`.
- Aplicar migración: `dotnet ef database update`.
- Alternativa: aplicar `IQ-DB.sql` manualmente y usar `EnsureCreated()` solo en desarrollo.

## Fase 3 — Corregir registro de servicios
- Cambiar registro de repositorios en `Program.cs` de `AddSingleton` a `AddScoped`.
- Asegurar que el ciclo de vida del DbContext y los repositorios es compatible.

## Fase 4 — Connection string segura
- Mover credenciales de conexión a variables de entorno o `dotnet user-secrets`.
- Usar `builder.Configuration.GetConnectionString("DefaultConnection")`.

## Fase 5 — Implementar DTOs de respuesta
- Crear DTOs para usuarios y posts que no expongan contraseñas ni datos sensibles.
- Actualizar los controllers para retornar DTOs en vez de entidades de dominio.

## Fase 6 — Manejo de errores HTTP
- Retornar `404 NotFound` cuando no se encuentre una entidad.
- Retornar `400 BadRequest` para datos inválidos.
- Agregar validaciones básicas en los handlers y controllers.

## Fase 7 — Endpoints de gestión de Tags
- Crear `TagController` con endpoints GET y POST.
- Implementar `ITagRepository` y su versión PostgreSQL.
- Agregar handlers CQRS para Tags.

## Fase 8 — Actualizar y ampliar tests
- Actualizar referencia de tests a `IndieQuest-Api_with_PosGre`.
- Agregar tests de integración para los repositorios PostgreSQL.
- Usar EF Core InMemory o Testcontainers para pruebas automatizadas.

---

## Archivos clave a modificar
- `IndieQuest-Api_with_PosGre/Program.cs`
- `IndieQuest-Api_with_PosGre/Infrastructure/IndieQuestDbContext.cs`
- `IndieQuest-Api_with_PosGre/IQ-Api.csproj`
- `IndieQuest-Api_with_PosGre/appsettings.json`
- `IndieQuest-Api_with_PosGre/Controllers/`
- `IndieQuest-Test/IndieQuest.Test.csproj`

---

## Verificación final
1. `dotnet build` sin errores.
2. `dotnet ef database update` crea las tablas correctamente.
3. `dotnet run` + Swagger: CRUD de Users y Posts funciona contra PostgreSQL.
4. `dotnet test` con los tests de integración pasando.

---

## Mejoras recomendadas
- Implementar hash de contraseñas (ej: BCrypt).
- Añadir autenticación y autorización (JWT).
- Mejorar la validación de datos (FluentValidation).
- Agregar logging estructurado.
- Restringir CORS en producción.
