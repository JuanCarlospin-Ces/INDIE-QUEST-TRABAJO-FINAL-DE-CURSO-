# Pruebas de Interfaz Gráfica - IndieQuest

---

## 📋 Autenticación

- **Registro de nuevo usuario**
  1. Ir a pantalla de registro
  2. Completar: Username, Email, Password, Confirmar Password
  3. Click en "Register"
  4. Verificar redirección a pantalla de login

- **Login exitoso**
  1. Ir a pantalla de login
  2. Ingresar username y password
  3. Click en "Login"
  4. Verificar redirección a pantalla principal (feed)

- **Login fallido**
  1. Ir a pantalla de login
  2. Ingresar credenciales incorrectas
  3. Click en "Login"
  4. Verificar mensaje de error

- **Logout**
  1. Estar en pantalla principal autenticado
  2. Click en "Logout"
  3. Verificar redirección a pantalla de login

---

## 👤 Gestión de Perfil

- **Ver listado de usuarios**
  1. Click en "Users" en navegación
  2. Esperar carga de lista
  3. Scroll hacia abajo
  4. Verificar que se cargan más usuarios automáticamente

- **Filtrar usuarios por disponibilidad**
  1. Ir a pantalla de usuarios
  2. Localizar checkbox "Available for work only"
  3. Click en el checkbox
  4. Verificar que se actualiza el listado
  5. Desmarcar checkbox
  6. Verificar que se muestran todos los usuarios

- **Ver perfil de usuario**
  1. Ir a pantalla de usuarios
  2. Click en el nombre de cualquier usuario
  3. Verificar que se muestra: foto, username, biografía, disponibilidad, posts

- **Editar perfil propio**
  1. Ir a pantalla de editar perfil
  2. Modificar: username, bio, email, disponibilidad para trabajar
  3. Click en "Save Changes"
  4. Verificar que cambios se reflejan inmediatamente

- **Subir foto de perfil**
  1. Ir a pantalla de editar perfil
  2. Click en "Choose File" en sección de foto
  3. Seleccionar archivo de imagen
  4. Click en "Upload"
  5. Verificar que foto se actualiza en el perfil

---

## 📝 Posts

- **Crear un post**
  1. Click en "Compose" en navegación
  2. Rellenar título y descripción
  3. Click en "Post"
  4. Verificar que aparece en pantalla principal

- **Ver feed de posts**
  1. Ir a pantalla principal
  2. Verificar que se muestra lista de posts
  3. Scroll hacia abajo
  4. Verificar que se cargan más posts automáticamente

- **Ver detalle de un post**
  1. Ir a pantalla principal
  2. Click en un post
  3. Verificar que se muestra información completa: título, descripción, autor, fecha

- **Editar un post**
  1. Ir a detalle del post propio
  2. Click en "Edit"
  3. Modificar título y descripción
  4. Click en "Save"
  5. Verificar que cambios se guardan

- **Eliminar un post**
  1. Ir a detalle del post propio
  2. Click en "Delete"
  3. Confirmar en popup
  4. Verificar que post desaparece del feed

- **Ver posts de un usuario específico**
  1. Ir a perfil de un usuario
  2. Scroll hacia abajo para ver posts
  3. Verificar que se muestran todos los posts del usuario

---

## 🔍 Búsqueda

- **Buscar usuarios**
  1. Click en "Search" en navegación
  2. Escribir nombre de usuario
  3. Presionar Enter
  4. Verificar que aparecen usuarios coincidentes

- **Buscar posts**
  1. Ir a pantalla de búsqueda
  2. Click en pestaña "Posts"
  3. Escribir palabra clave
  4. Verificar que aparecen posts coincidentes

- **Filtrar búsqueda de usuarios por disponibilidad**
  1. Ir a pantalla de búsqueda
  2. Escribir nombre de usuario
  3. Click en checkbox "Available for work only"
  4. Verificar que se muestran solo usuarios disponibles

- **Buscar por tags**
  1. Ir a pantalla de búsqueda
  2. Click en pestaña "Tags"
  3. Escribir nombre de tag
  4. Verificar que aparecen posts con ese tag

---

## 📊 Paginación

- **Paginación en lista de usuarios**
  1. Ir a pantalla de usuarios
  2. Scroll al final de la página
  3. Verificar que se cargan más usuarios automáticamente
  4. Verificar que se agregan sin duplicados

- **Paginación en feed de posts**
  1. Ir a pantalla principal
  2. Scroll hacia abajo múltiples veces
  3. Verificar que se cargan más posts automáticamente

---

## 🎨 Interfaz

- **Responsividad en móvil**
  1. Abrir DevTools (F12)
  2. Seleccionar vista móvil (375x667)
  3. Navegar a pantalla de usuarios
  4. Verificar que layout se adapta correctamente
  5. Verificar que botones son clickeables

- **Responsividad en tablet**
  1. Abrir DevTools
  2. Seleccionar vista tablet (768x1024)
  3. Navegar a pantalla de usuarios y feed
  4. Verificar que layout se adapta correctamente

- **Carga de imágenes**
  1. Ir a pantalla de usuarios
  2. Observar carga de fotos de perfil
  3. Verificar que imágenes cargan correctamente
  4. Verificar que no hay imágenes rotas

---

## ⚡ Rendimiento

- **Carga inicial de página principal**
  1. Abrir DevTools → Network
  2. Limpiar cache (Ctrl+Shift+Delete)
  3. Ir a pantalla principal
  4. Medir tiempo de carga
  5. Verificar que carga en menos de 3 segundos

- **Rendimiento con muchos usuarios**
  1. Ir a pantalla de usuarios
  2. Scroll hasta cargar 100+ usuarios
  3. Verificar que scroll sigue siendo fluido
  4. Verificar que no hay lag

---

## 🚨 Errores

- **Usuario no encontrado**
  1. Navegar a pantalla de perfil de usuario inexistente
  2. Verificar que muestra mensaje de error

- **Post no encontrado**
  1. Navegar a pantalla de detalle de post inexistente
  2. Verificar que muestra mensaje de error

- **API no disponible**
  1. Detener servidor de API
  2. Intentar cargar pantalla de usuarios
  3. Verificar que muestra mensaje de error amigable

---

## ✅ Checklist

- [ ] Registro
- [ ] Login exitoso
- [ ] Login fallido
- [ ] Logout
- [ ] Ver usuarios
- [ ] Filtrar por disponibilidad
- [ ] Ver perfil
- [ ] Editar perfil
- [ ] Subir foto
- [ ] Crear post
- [ ] Ver feed
- [ ] Detalle de post
- [ ] Editar post
- [ ] Eliminar post
- [ ] Posts de usuario
- [ ] Buscar usuarios
- [ ] Buscar posts
- [ ] Filtrar búsqueda
- [ ] Buscar tags
- [ ] Paginación usuarios
- [ ] Paginación posts
- [ ] Responsividad móvil
- [ ] Responsividad tablet
- [ ] Carga imágenes
- [ ] Carga inicial
- [ ] Rendimiento
- [ ] Usuario no encontrado
- [ ] Post no encontrado
- [ ] API no disponible
