# Consideraciones y Recomendaciones de Testing para Migración a PostgreSQL

Este documento resume los cambios y recomendaciones para adaptar y mejorar los tests al migrar la API de Indie Quest de almacenamiento en memoria a una versión con PostgreSQL y Entity Framework Core.

---

## 1. Cambios principales entre ambas APIs

- **Tipo de ID:**
  - En memoria: `string` (GUID)
  - PostgreSQL: `int` (autoincremental)
- **Relaciones:**
  - En memoria: arrays embebidos
  - PostgreSQL: tablas intermedias (`UserPost`, `PostTag`)

---

## 2. Impacto en los diferentes tipos de tests

| Nivel de test      | Qué cambia                                                        | Impacto   |
|--------------------|-------------------------------------------------------------------|-----------|
| **Unit**           | Tipos de ID (`string` → `int`) en mocks y modelos                 | Bajo      |
| **Acceptance**     | Tipos de ID + eliminar `UserId` del Command                      | Bajo-Medio|
| **Integration**    | Usar `PostgreSqlRepository` + base de datos real                 | Alto      |
| **End-to-End**     | Referenciar proyecto PostgreSQL + base de datos disponible        | Alto      |

### Detalles por tipo de test

#### Unit Tests
- Usan Moq sobre interfaces, no dependen de la infraestructura.
- Solo hay que adaptar los tipos de ID y modelos usados en los mocks.

#### Acceptance Tests
- Igual que los unitarios, pero los comandos de creación ya no llevan `UserId` (lo genera la BD).
- Verifica solo los datos relevantes (no el ID).

#### Integration Tests
- Los actuales prueban los repositorios en memoria.
- Para PostgreSQL, crea tests equivalentes usando `PostgreSqlUserRepository` y una base de datos real (idealmente efímera).

#### End-to-End Tests
- Cambia la referencia del proyecto de API en memoria a la versión PostgreSQL.
- Asegúrate de tener una base de datos disponible para los tests.

---

## 3. Recomendaciones de nuevos tests para PostgreSQL

1. **Tests de integración con Testcontainers:**
   - Usa `Testcontainers.PostgreSQL` para levantar una BD real durante los tests.
   - Permite pruebas limpias y portables.

2. **Test de relaciones N:M:**
   - Verifica que las relaciones (`Makes_MadeBy`, `Has_Tag`) funcionan correctamente en la BD.

3. **Test de queries complejas:**
   - Ejemplo: `GetPostsByUserIdAsync` debe funcionar correctamente con JOINs reales.

4. **Test de restricciones de unicidad y claves foráneas:**
   - Verifica que la BD lanza excepciones al insertar datos duplicados o inválidos.

5. **Test de borrado en cascada:**
   - Comprueba que al borrar un usuario, sus relaciones en tablas intermedias se eliminan automáticamente.

---

## 4. Consejos generales

- Mantén los tests unitarios y de aceptación desacoplados de la infraestructura.
- Usa bases de datos efímeras para integración y E2E (Testcontainers o similar).
- No olvides limpiar los datos entre tests para evitar efectos colaterales.
- Documenta las diferencias de comportamiento entre la versión en memoria y la de PostgreSQL.

---

> **Este documento te servirá como referencia para adaptar y mejorar tu suite de tests al migrar a una infraestructura real con PostgreSQL.**
