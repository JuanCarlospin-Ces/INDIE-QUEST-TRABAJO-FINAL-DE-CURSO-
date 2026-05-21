# Errores Detectados y Soluciones Aplicadas - API Tests

---

**Error:** Filter availableForWork causaba NullReferenceException en GetAllUsersQueryHandler
**Solución:** Implementar validación con `if (availableForWork.HasValue)` antes de aplicar el filtro en la query LINQ

---

**Error:** PostgreSqlUserRepository no filtraba usuarios correctamente por disponibilidad
**Solución:** Agregar condición `query = query.Where(u => u.AvailableForWork == availableForWork.Value)` en GetAllUsersPagedAsync()

---

**Error:** UserController no pasaba el parámetro availableForWork al handler
**Solución:** Agregar parámetro `[FromQuery] bool? availableForWork = null` al método GetAllUsers y pasarlo al handler

---

**Error:** client.js no incluía el filtro en las queries a la API
**Solución:** Implementar construcción condicional de query string: `if (availableForWork !== null) { query += '&availableForWork=${availableForWork}' }`

---

**Error:** UsersPage no tenía estado para el checkbox de disponibilidad
**Solución:** Agregar estado `const [showAvailableOnly, setShowAvailableOnly] = useState(false)` y pasarlo a getUsersPaged()

---

**Error:** El filtro en UsersPage no se reflejaba al cambiar el checkbox
**Solución:** Implementar useEffect con dependencia `[showAvailableOnly]` que recarga la lista cuando cambia el estado del checkbox

---

**Error:** La paginación en UsersPage no reseteaba al cambiar el filtro
**Solución:** Resetear `setPage(1)` y `setUsers([])` dentro del useEffect cuando cambia showAvailableOnly

---

**Error:** SearchPage no tenía visibilidad del checkbox para filtrar usuarios
**Solución:** Agregar el checkbox en SearchPage con el mismo estado showAvailableOnly que UsersPage

---

**Error:** El filtro en SearchPage solo funcionaba cuando había búsqueda activa
**Solución:** Cambiar la condición para mostrar el checkbox a `showUsers || tab === 'all'` en lugar de verificar si había query

---

**Error:** getAllUsers en client.js no aceptaba parámetro availableForWork
**Solución:** Modificar función para recibir parámetro `availableForWork = null` y construir query string condicional

---

**Error:** getUsersPaged no pasaba el filtro a la API cuando era null
**Solución:** Cambiar lógica para solo agregar parámetro a la query string si `availableForWork !== null`

---

**Error:** InfiniteScroll en UsersPage llamaba loadMore() múltiples veces simultáneamente
**Solución:** Agregar flag `loadingMore` y validar `if (loadingMore || page >= totalPages) return` antes de cargar

---

**Error:** El IntersectionObserver en UsersPage no disparaba al scroll
**Solución:** Implementar correctamente el observer con `rootMargin: '300px'` para cargar 300px antes del final

---

**Error:** Los usuarios cargados por infinite scroll tenían duplicados
**Solución:** Usar spread operator `[...prev, ...paged.data]` para append correctamente en setUsers

---

**Error:** La paginación en SearchPage no reseteaba cuando cambiaba el filtro
**Solución:** Agregar dependencia `[showAvailableOnly]` al useEffect que carga datos de usuarios y posts

---

**Error:** El checkbox de filtro no se desmarcaba correctamente en SearchPage
**Solución:** Implementar onChange handler que actualiza correctamente el estado: `onChange={(e) => setShowAvailableOnly(e.target.checked)}`

---

**Error:** Las funciones getUsersPaged y getAllUsers tenían lógica de query string inconsistente
**Solución:** Unificar patrón de construcción con variable `let query = ...` y condicional para agregar parámetros

---

**Error:** El filtro causaba re-renders infinitos en SearchPage
**Solución:** Agregar correctamente showAvailableOnly como dependencia en useEffect para evitar bucles

---

**Error:** Los usuarios filtrados no se actualizaban al cambiar de "All" a "Users" tab
**Solución:** Implementar estado independiente para mostrar filtro: `const showUsers = tab === 'all' || tab === 'users'`

---

**Error:** Promise.all en SearchPage se rechazaba si getAllUsers o getAllPosts fallaban parcialmente
**Solución:** Agregar validación `Array.isArray(u) ? u : []` después de resolver promises para manejar respuestas inválidas

---
