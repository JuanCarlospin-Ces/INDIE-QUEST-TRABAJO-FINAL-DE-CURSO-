# Errores Detectados y Soluciones Aplicadas - UI

---

**Error:** El checkbox "Available for work only" no actualizaba la lista de usuarios en UsersPage
**Solución:** Implementar useEffect con dependencia `[showAvailableOnly]` que llama a `getUsersPaged(1, PAGE_SIZE, showAvailableOnly ? true : null)`

---

**Error:** La paginación no reseteaba al marcar/desmarcar el checkbox en UsersPage
**Solución:** Resetear estados `setPage(1)` y `setUsers([])` al inicio del useEffect cuando cambia `showAvailableOnly`

---

**Error:** El infinite scroll cargaba la misma página múltiples veces simultaneamente
**Solución:** Agregar validación `if (loadingMore || page >= totalPages) return` en la función `loadMore()` antes de hacer la request

---

**Error:** El IntersectionObserver no detectaba cuando usuario hacía scroll al final en UsersPage
**Solución:** Configurar correctamente el observer con `rootMargin: '300px'` para cargar más usuarios 300px antes de llegar al sentinel

---

**Error:** Los usuarios duplicados aparecían al cargar más con infinite scroll
**Solución:** Usar append correcto en setUsers con spread operator: `setUsers((prev) => [...prev, ...(paged?.data ?? [])])`

---

**Error:** El checkbox en SearchPage no reseteaba al cambiar de tab
**Solución:** Agregar estado independiente `const [showAvailableOnly, setShowAvailableOnly] = useState(false)` en SearchPage

---

**Error:** El checkbox de filtro solo era visible cuando había búsqueda activa en SearchPage
**Solución:** Cambiar la condición de render a `showUsers || tab === 'all'` en lugar de verificar si `query` tiene contenido

---

**Error:** Los usuarios filtrados en SearchPage no se actualizaban al marcar el checkbox
**Solución:** Agregar dependencia `[showAvailableOnly]` al useEffect que carga datos: `useEffect(() => { ... }, [showAvailableOnly])`

---

**Error:** La búsqueda de usuarios en SearchPage se hacía lenta con muchos usuarios en memoria
**Solución:** Implementar useMemo para cachear resultados filtrados: `const matchedUsers = useMemo(() => { ... }, [users, q])`

---

**Error:** El input de búsqueda en SearchPage no se limpiaba al hacer click en "clear"
**Solución:** Implementar button con onClick handler que ejecuta `setQuery('')`

---

**Error:** Las fotos de perfil no cargaban correctamente en la grilla de usuarios
**Solución:** Verificar que el componente Avatar recibe correctamente el prop con la ruta de imagen desde pickField()

---

**Error:** El spinner de carga aparecía aunque ya había datos cargados en UsersPage
**Solución:** Cambiar lógica de render para mostrar spinner solo cuando `loading && users.length === 0`

---

**Error:** El error no se mostraba al fallar la carga de usuarios en UsersPage
**Solución:** Agregar componente `<ErrorBox error={error} />` en el JSX para mostrar errores capturados en el try-catch

---

**Error:** El PageHeader no se rendereaba correctamente en UsersPage
**Solución:** Usar componente `<PageHeader title="Users" subtitle="People in the IndieQuest community" />`

---

**Error:** Los datos de usuario tenían inconsistencia de casos (userId vs UserId)
**Solución:** Implementar función `pickField(u, 'userId', 'UserId')` para manejar ambas variaciones de nombre de propiedad

---

**Error:** El estado de la búsqueda en SearchPage no se sincronizaba con la URL
**Solución:** Implementar useEffect que actualiza URL params: `setSearchParams({q: query, tab: tab}, { replace: true })`

---

**Error:** Los tabs en SearchPage no cambiaban el contenido mostrado
**Solución:** Implementar variables booleanas `const showUsers = tab === 'all' || tab === 'users'` y usarlas para condicionar el render

---

**Error:** Los posts en SearchPage no se cargaban cuando no había búsqueda
**Solución:** Cambiar Promise.all para cargar sempre getAllPosts en SearchPage y filtrar en useMemo

---

**Error:** El autoFocus en el input de búsqueda de SearchPage no funcionaba
**Solución:** Agregar atributo `autoFocus` al input element

---

**Error:** La paginación infinita se detenía sin motivo aparente en UsersPage
**Solución:** Validar que `totalPages` se actualiza correctamente en setTotalPages y que `page < totalPages` en la condición de loadMore

---

**Error:** El cleanup del IntersectionObserver no se ejecutaba en UsersPage
**Solución:** Retornar función cleanup en el useEffect: `return () => observer.disconnect()`

---

**Error:** Las validaciones de campo en formularios mostraban errores incompletos
**Solución:** Agregar componente ErrorBox que muestra `error.message` del catch en los useEffect

---

**Error:** El estado de página se reiniciaba sin razón al cambiar availableForWork
**Solución:** Mover la inicialización de `page` y `totalPages` dentro del useEffect que depende de `[showAvailableOnly]`

---

**Error:** El usuario podía hacer click múltiples veces en usuarios con lag de red
**Solución:** Agregar atributo `disabled` en botones/links cuando `loading` o `loadingMore` es true

---
