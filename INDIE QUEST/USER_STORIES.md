# Historias de Usuario - IndieQuest

## 📖 Descripción General

Las siguientes historias de usuario documentan las funcionalidades principales de la aplicación IndieQuest, una plataforma social diseñada para que desarrolladores independientes compartan proyectos, se conecten y colaboren.

---

## 👤 Roles Identificados

- **Usuario Independiente** - Desarrollador/creador que comparte proyectos
- **Buscador de Talento** - Usuario que busca colaboradores o desarrolladores
- **Comunidad IndieQuest** - Conjunto de usuarios interconectados

---

## 📚 Historias de Usuario

### 1. Gestión de Perfil de Usuario

#### US-001: Registrarse en la plataforma
**Como** un nuevo usuario  
**Quiero** crear una cuenta en IndieQuest  
**Para** poder compartir mis proyectos y conectarme con otros desarrolladores

**Criterios de Aceptación:**
- El usuario puede registrarse con nombre de usuario, email y contraseña
- El email debe ser único en el sistema
- La contraseña debe ser almacenada de forma segura (hasheada)
- Se recibe confirmación del registro exitoso
- El usuario es redirigido al perfil para completar información adicional

**Datos Involucrados:**
- username (único)
- email (único)
- password (hasheada)
- dateOfRegistration (automático)

---

#### US-002: Actualizar perfil de usuario
**Como** usuario registrado  
**Quiero** actualizar mi información de perfil  
**Para** mantener mi información actualizada y visible en la comunidad

**Criterios de Aceptación:**
- Puedo editar mi nombre de usuario
- Puedo editar mi biografía/descripción
- Puedo editar mi email
- Puedo cambiar mi contraseña
- Puedo marcar si estoy disponible para trabajar
- Los cambios se guardan inmediatamente en la BD
- Recibo confirmación de actualización exitosa

**Datos Involucrados:**
- username
- userBio
- email
- availableForWork (booleano)
- password

---

#### US-003: Subir foto de perfil
**Como** usuario registrado  
**Quiero** subir una foto para mi perfil  
**Para** que otros usuarios puedan reconocerme visualmente

**Criterios de Aceptación:**
- Puedo seleccionar una imagen de mi computadora (máx 50MB)
- La imagen se sube al servidor
- La imagen se almacena en la ruta `IndieQuest-LocalData/user/{id}/{filename}`
- Se reemplaza la foto anterior si existe
- Mi perfil muestra la foto actualizada
- Se valida el tipo de archivo (imagen)

**Datos Involucrados:**
- userProfilePicture (ruta relativa)

---

#### US-004: Ver perfil de usuario
**Como** usuario  
**Quiero** ver el perfil de otros usuarios  
**Para** conocer más sobre desarrolladores en la plataforma

**Criterios de Aceptación:**
- Puedo ver el nombre de usuario
- Puedo ver la biografía del usuario
- Puedo ver su foto de perfil
- Puedo ver si está disponible para trabajar
- Puedo ver sus posts publicados
- La información se carga correctamente desde la BD

**Datos Mostrados:**
- username
- userBio
- userProfilePicture
- availableForWork
- dateOfRegistration

---

#### US-005: Ver listado de usuarios
**Como** usuario  
**Quiero** ver un listado de otros usuarios en la plataforma  
**Para** descubrir desarrolladores potenciales para colaborar

**Criterios de Aceptación:**
- Puedo ver un grid con tarjetas de usuarios
- Cada tarjeta muestra: foto, nombre, disponibilidad, biografía
- La lista se pagina (10 usuarios por página)
- Puedo navegar entre páginas
- Los usuarios se cargan automáticamente al hacer scroll
- La página es responsive

**Paginación:**
- pageNumber: número de página
- pageSize: 10 usuarios por defecto
- totalCount: total de usuarios
- totalPages: páginas totales

---

#### US-006: Filtrar usuarios por disponibilidad para trabajar
**Como** usuario buscando colaboradores  
**Quiero** filtrar el listado de usuarios por disponibilidad  
**Para** encontrar rápidamente desarrolladores que puedan colaborar conmigo

**Criterios de Aceptación:**
- Existe un checkbox "Available for work only" en el listado de usuarios
- El checkbox es visible en la página de usuarios
- El checkbox es visible en la página de búsqueda
- Al activar el filtro, se muestran solo usuarios con availableForWork = true
- La paginación se recalcula correctamente con el filtro
- Al desactivar el filtro, se muestran todos los usuarios nuevamente
- El filtro persiste durante la sesión

**Técnica:**
- Parámetro query: `?availableForWork=true`
- Filtrado en backend en repositorio
- Frontend envía parámetro al cargar datos

---

#### US-007: Buscar usuarios por nombre
**Como** usuario  
**Quiero** buscar otros usuarios por nombre de usuario  
**Para** encontrar a colegas específicos que conozco

**Criterios de Aceptación:**
- Existe barra de búsqueda en la página de Search
- Puedo escribir el nombre de usuario
- La búsqueda es case-insensitive
- Se muestran resultados coincidentes en tiempo real
- Puedo aplicar filtro de disponibilidad mientras busco
- Los resultados incluyen foto, nombre, disponibilidad y biografía

**Búsqueda:**
- Búsqueda por username
- Match parcial
- Se combina con filtro de availableForWork

---

#### US-008: Eliminar cuenta de usuario
**Como** usuario  
**Quiero** eliminar mi cuenta de la plataforma  
**Para** que mis datos se eliminen del sistema

**Criterios de Aceptación:**
- Puedo solicitar la eliminación de mi cuenta
- Se requiere confirmación antes de eliminar
- La eliminación es irreversible
- Se elimina el usuario y todos sus datos asociados
- Se elimina la carpeta de perfil del usuario

**Datos Eliminados:**
- Registro de usuario
- Folder: `IndieQuest-LocalData/user/{id}/`

---

### 2. Gestión de Posts

#### US-009: Crear un post
**Como** usuario registrado  
**Quiero** crear un post para compartir mi proyecto o idea  
**Para** que otros usuarios vean lo que estoy trabajando

**Criterios de Aceptación:**
- Puedo escribir un título para el post
- Puedo escribir una descripción detallada
- El post se asocia automáticamente a mi usuario
- Se registra la fecha y hora de creación
- El post se marca como activo por defecto
- Recibo confirmación de creación exitosa
- El post es visible inmediatamente en mi perfil

**Datos Involucrados:**
- title (requerido)
- description (requerido)
- postUserId (automático)
- dateOfCreation (automático)
- active = true (por defecto)

---

#### US-010: Editar un post
**Como** autor del post  
**Quiero** editar el contenido de mis posts  
**Para** corregir errores o actualizar información

**Criterios de Aceptación:**
- Solo el autor puede editar su post
- Puedo editar el título
- Puedo editar la descripción
- Los cambios se guardan inmediatamente
- Recibo confirmación de actualización exitosa
- La fecha de creación no cambia

**Validaciones:**
- Verificación de propiedad del post
- Campos no vacíos

---

#### US-011: Eliminar un post
**Como** autor del post  
**Quiero** eliminar posts que no quiero que sigan visibles  
**Para** mantener mi perfil actualizado

**Criterios de Aceptación:**
- Solo el autor puede eliminar su post
- El post se marca como inactivo (eliminación lógica)
- El post desaparece de los listados públicos
- Recibo confirmación de eliminación exitosa
- El post no se elimina completamente de la BD (por auditoría)

**Implementación:**
- Campo `active = false`
- No se elimina el registro

---

#### US-012: Ver listado de posts
**Como** usuario  
**Quiero** ver un feed con los posts más recientes de la comunidad  
**Para** descubrir lo que están trabajando otros desarrolladores

**Criterios de Aceptación:**
- Veo un feed con posts de todos los usuarios
- Cada post muestra: título, descripción, autor, fecha, foto del autor
- Los posts están paginados (10 por página)
- Puedo navegar entre páginas
- Los posts se cargan automáticamente al hacer scroll
- Solo se muestran posts activos

**Paginación:**
- pageNumber, pageSize = 10
- totalCount, totalPages calculados

---

#### US-013: Ver un post específico
**Como** usuario  
**Quiero** ver el detalle completo de un post  
**Para** conocer más detalles sobre un proyecto específico

**Criterios de Aceptación:**
- Veo el título y descripción completa del post
- Veo la información del autor (nombre, foto, biografía)
- Veo la fecha de creación
- Puedo navegar al perfil del autor
- Si el post está inactivo, solo el autor lo ve

**Datos Mostrados:**
- title, description
- author info
- dateOfCreation
- active status

---

#### US-014: Ver posts de un usuario específico
**Como** usuario  
**Quiero** ver todos los posts de un desarrollador específico  
**Para** conocer todos sus proyectos

**Criterios de Aceptación:**
- Al entrar en el perfil de un usuario, veo sus posts
- Se muestran todos sus posts activos
- Los posts están ordenados por fecha (más reciente primero)
- Puedo navegar por los posts (paginación o scroll)
- Si soy el autor, puedo editar o eliminar mis posts

**Endpoint:**
- `GET /api/Post/user/{userId}`

---

### 3. Búsqueda y Descubrimiento

#### US-015: Buscar en toda la plataforma
**Como** usuario  
**Quiero** buscar usuarios, posts y tags en la plataforma  
**Para** encontrar rápidamente lo que me interesa

**Criterios de Aceptación:**
- Existe una barra de búsqueda en la navegación principal
- Puedo escribir una búsqueda
- Se muestran resultados de tres categorías: Usuarios, Posts, Tags
- Cada categoría tiene una pestaña para filtrar
- Los resultados son case-insensitive
- Puedo filtrar por disponibilidad mientras busco

**Búsqueda Por:**
- Usuarios: nombre de usuario
- Posts: título y descripción
- Tags: nombre del tag

---

#### US-016: Filtrar resultados de búsqueda
**Como** usuario buscando  
**Quiero** filtrar los resultados de búsqueda por tipo  
**Para** ver solo lo que me interesa

**Criterios de Aceptación:**
- Pestañas: "All", "Users", "Posts", "Tags"
- Al hacer clic en una pestaña, se muestran solo ese tipo de resultado
- El filtro de disponibilidad aplica solo a usuarios
- Los resultados se filtran en tiempo real

**Categorías:**
- All: todos los resultados
- Users: solo usuarios
- Posts: solo posts
- Tags: posts que contienen el tag

---

### 4. Autenticación y Sesión

#### US-017: Iniciar sesión
**Como** usuario registrado  
**Quiero** iniciar sesión con mis credenciales  
**Para** acceder a la plataforma

**Criterios de Aceptación:**
- Existe página de login
- Puedo ingresar username y password
- Se validan las credenciales contra la BD
- Si las credenciales son correctas, se inicia sesión
- Si son incorrectas, se muestra un mensaje de error
- Soy redirigido a la página principal después del login

**Validaciones:**
- Username requerido
- Password requerido
- Verificación en BD

---

#### US-018: Cerrar sesión
**Como** usuario autenticado  
**Quiero** cerrar mi sesión  
**Para** salir de la plataforma de forma segura

**Criterios de Aceptación:**
- Existe botón de logout en la navegación
- Al hacer clic, se cierra la sesión
- Se limpia el contexto de autenticación
- Soy redirigido a la página de login
- No puedo acceder a páginas protegidas sin autenticarme

---

### 5. Características Técnicas

#### US-019: Paginar resultados
**Como** aplicación  
**Quiero** paginar los listados de usuarios y posts  
**Para** optimizar el rendimiento y la experiencia del usuario

**Criterios de Aceptación:**
- Los listados se dividen en páginas de 10 elementos
- Se puede especificar el número de página (pageNumber)
- Se puede especificar el tamaño de página (pageSize)
- La respuesta incluye: data, pageNumber, pageSize, totalCount, totalPages
- El cálculo de totalPages es correcto (Math.Ceiling(totalCount / pageSize))

**Respuesta:**
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5
}
```

---

#### US-020: Subir archivos multimedia
**Como** usuario  
**Quiero** subir fotos a mis posts  
**Para** acompañar mis proyectos con contenido visual

**Criterios de Aceptación:**
- Puedo subir imágenes desde mis posts
- Se valida el tipo de archivo (imagen)
- Se valida el tamaño máximo (500MB)
- Se genera una ruta segura para almacenar el archivo
- Se puede acceder al archivo posteriormente
- Se maneja apropiadamente errores de upload

**Validaciones:**
- Tipo MIME validado
- Tamaño máximo: 500MB
- Ruta segura: `IndieQuest-LocalData/user/{id}/`

---

## 📊 Matriz de Relaciones

| Historia | Usuario | Post | Búsqueda | Auth | Técnica |
|----------|---------|------|----------|------|---------|
| US-001 | ✅ | | | ✅ | |
| US-002 | ✅ | | | | |
| US-003 | ✅ | | | | ✅ |
| US-004 | ✅ | | | | |
| US-005 | ✅ | | ✅ | | ✅ |
| US-006 | ✅ | | ✅ | | |
| US-007 | ✅ | | ✅ | | |
| US-008 | ✅ | | | | |
| US-009 | | ✅ | | ✅ | |
| US-010 | | ✅ | | | |
| US-011 | | ✅ | | | |
| US-012 | | ✅ | ✅ | | ✅ |
| US-013 | ✅ | ✅ | ✅ | | |
| US-014 | ✅ | ✅ | | | ✅ |
| US-015 | ✅ | ✅ | ✅ | | ✅ |
| US-016 | ✅ | ✅ | ✅ | | |
| US-017 | ✅ | | | ✅ | |
| US-018 | ✅ | | | ✅ | |
| US-019 | ✅ | ✅ | ✅ | | ✅ |
| US-020 | ✅ | ✅ | | | ✅ |

---

## 🎯 Priorizaciones

### Priority 1 - Crítico (MVP)
- US-001: Registro ✅
- US-017: Login ✅
- US-009: Crear post ✅
- US-012: Ver posts ✅
- US-005: Ver usuarios ✅

### Priority 2 - Alto
- US-002: Editar perfil ✅
- US-010: Editar post ✅
- US-004: Ver perfil ✅
- US-015: Buscar ✅
- US-006: Filtro disponibilidad ✅ (Nueva)

### Priority 3 - Medio
- US-003: Foto de perfil ✅
- US-007: Buscar por nombre ✅
- US-013: Ver post detalle ✅
- US-014: Posts por usuario ✅
- US-016: Filtrar búsqueda ✅

### Priority 4 - Bajo
- US-008: Eliminar cuenta
- US-011: Eliminar post
- US-018: Logout
- US-019: Paginación (técnica)
- US-020: Subir archivos

---

## 📝 Notas de Implementación

### Patrones Utilizados
- **CQRS:** Commands para escritura (Create, Update, Delete), Queries para lectura (Get)
- **Clean Architecture:** Separación en capas (Controllers, Handlers, Repositories)
- **In-Memory DB:** Para testing rápido sin BD real

### Validaciones Generales
- Campos requeridos no pueden estar vacíos
- Emails deben ser únicos
- Usernames deben ser únicos
- Contraseñas hasheadas (no en texto plano)
- Archivos validados por tipo y tamaño

### Seguridad
- Autenticación requerida para modificar datos
- Verificación de propiedad (solo autor puede editar su contenido)
- Hasheado de contraseñas
- Validación de entrada en servidor

---

## 🚀 Desarrollo Futuro

- [ ] Sistema de comentarios en posts
- [ ] Sistema de likes/favoritos
- [ ] Notificaciones en tiempo real
- [ ] Mensajería privada entre usuarios
- [ ] Sistema de colaboración en proyectos
- [ ] Portafolio visual de proyectos
- [ ] Integración con repositorios Git
- [ ] Sistema de calificaciones/reviews
- [ ] Badges y logros
- [ ] Newsletter/actualizaciones

---

## 📞 Contacto y Aclaraciones

Para aclaraciones sobre historias de usuario, contactar al equipo de product management.

**Última actualización:** Mayo 15, 2026  
**Versión:** 1.0
