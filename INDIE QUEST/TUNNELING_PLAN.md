# Plan de Despliegue - IndieQuest con Dominio Personalizado

## ✅ SOLUCIÓN IMPLEMENTADA

**Frontend:** Vercel (indiequest.vercel.app o indiequest.com)  
**API:** Local en tu máquina, expuesta con localtunnel  
**React Router:** ✅ Funcionando con rewrite automático  
**HTTPS:** ✅ Automático en Vercel  
**Dominio personalizado:** ✅ Listo para configurar  

---

## Arquitectura Final

```
Tu Ordenador (Local)
├── npm run dev → http://localhost:5173 (Frontend)
├── dotnet run → http://localhost:5063 (API)
└── npm run tunnel:api → https://indiequest-api-xxxx.loca.lt (Exposición pública)

↓

Internet (Vercel)
├── https://indiequest.vercel.app (Frontend desplegado)
└── Lee variables de entorno → accede a API local vía tunnel
```

---

## Requisitos Críticos para React Router

✅ **SOLUCIONADO:** React Router necesita que **TODAS las rutas (`/home`, `/profile`, etc.) sirvan `index.html`**.

Configuración implementada:
- ✅ `vercel.json` con rewrite automático a `index.html`
- ✅ `vite.config.js` optimizado para producción
- ✅ CORS permitido en backend para orígenes remotos

---

## Opción 1: Vercel (Recomendada) ✅

### ¿Por qué Vercel?

- ✅ Manejo automático de React Router (detecta Next.js/Vite + SPA)
- ✅ Dominio personalizado con HTTPS automático
- ✅ Variables de entorno para API local
- ✅ Preview automáticas en PR
- ✅ Deploy con `git push` (CI/CD)
- ✅ Tier gratuita generosa

### Pasos de Implementación

**1. Conectar repositorio a Vercel**
- Ir a https://vercel.com/new
- Conectar GitHub/GitLab
- Seleccionar repositorio IndieQuest
- Vercel detectará automáticamente Vite

**2. Configurar variables de entorno en Vercel**

```env
VITE_API_BASE_URL=http://localhost:5063/api
# ⚠️ Vercel ejecuta en servidor remoto, pero desde el navegador 
# necesitaremos otra URL si la API está en otra máquina
```

**3. Configurar dominio personalizado**
- En Dashboard Vercel → Settings → Domains
- Añadir `indiequest.com`
- Seguir instrucciones de DNS
- HTTPS se genera automáticamente

**4. Configurar vite.config.js**

```js
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
  },
  build: {
    outDir: 'dist',  // Vercel busca esto
  }
});
```

**5. Crear vercel.json en raíz del proyecto**

```json
{
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ]
}
```

**6. Script en package.json (opcional, Vercel lo auto-detecta)**
```json
"scripts": {
  "dev": "vite",
  "build": "vite build",
  "preview": "vite preview"
}
```

---

## Opción 2: Netlify (Alternativa)

### Características

- ✅ Manejo automático de SPA (React Router)
- ✅ Dominio personalizado + HTTPS automático
- ✅ Deploy desde Git
- ✅ Functions para proxy a API local (si es necesario)

### Pasos

**1. Conectar en https://app.netlify.com**
- Seleccionar "New site from Git"
- Conectar GitHub
- Build command: `npm run build`
- Publish directory: `dist`

**2. Configurar dominio**
- Settings → Domain management
- Añadir custom domain `indiequest.com`
- Actualizar DNS

**3. Crear netlify.toml en raíz**

```toml
[build]
command = "npm run build"
publish = "dist"

[[redirects]]
from = "/*"
to = "/index.html"
status = 200
```

**4. Variables de entorno**
- En Netlify Dashboard → Environment
- `VITE_API_BASE_URL=http://localhost:5063/api`

---

## Opción 3: Servidor Propio con Node.js + Nginx (Control Total)

Si prefieres control completo del servidor:

### Instalación en Servidor

**1. Build la aplicación**
```bash
npm run build
# Genera carpeta `dist/`
```

**2. Usar servidor Express simple**

Crear `server.js` en raíz:
```js
import express from 'express';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const app = express();

// Servir archivos estáticos
app.use(express.static(path.join(__dirname, 'dist')));

// React Router: todas las rutas van a index.html
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'dist', 'index.html'));
});

app.listen(3000, () => {
  console.log('Server running on port 3000');
});
```

**3. Configurar Nginx como reverse proxy**

```nginx
server {
    listen 443 ssl http2;
    server_name indiequest.com;

    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;

    location / {
        proxy_pass http://localhost:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        
        # Para React Router
        try_files $uri $uri/ /index.html;
    }
}

# Redirect HTTP → HTTPS
server {
    listen 80;
    server_name indiequest.com;
    return 301 https://$server_name$request_uri;
}
```

**4. HTTPS Automático con Let's Encrypt**

```bash
sudo certbot certonly --standalone -d indiequest.com
# Nginx renueva automáticamente
```

---

## Conexión a API Local

### Escenario: API en máquina local, Frontend desplegado en nube

⚠️ **Problema:** El navegador no puede acceder directamente a `http://localhost:5063` desde Internet.

**Solución 1: Proxy en Vercel/Netlify (Functions)**

En Vercel, crear `api/proxy.js`:
```js
export default async function handler(req, res) {
  const apiUrl = `http://<tu-ip-local>:5063${req.url}`;
  
  const response = await fetch(apiUrl, {
    method: req.method,
    headers: req.headers,
    body: req.body
  });
  
  return res.status(response.status).json(await response.json());
}
```

Luego en `.env`:
```env
VITE_API_BASE_URL=/api/proxy
```

**Solución 2: Usar API Pública (Mejor)**

Desplegar la API (.NET) también en nube (Azure, AWS, etc.) con su propio dominio.

**Solución 3: Tunnel para API**

Usar `npx localtunnel` solo para la API:
```bash
npx localtunnel --port 5063
# Obtiene URL pública tipo https://xxxx.loca.lt

# En .env.production de Vercel:
VITE_API_BASE_URL=https://xxxx.loca.lt/api
```

---

## Plan de Implementación Detallado

### Fase 1: Preparar el proyecto para build de producción

**Archivo: `vite.config.js`**
```js
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
  },
  build: {
    outDir: 'dist',
    sourcemap: false,  // Opcional: reduce tamaño en producción
  }
});
```

**Archivo: `.env.production`**
```env
VITE_API_BASE_URL=http://localhost:5063/api
# O si está desplegada:
# VITE_API_BASE_URL=https://api.indiequest.com/api
```

### Fase 2: Configurar React Router para SPA

**En tu App.jsx/main.jsx, asegurar que React Router está bien configurado:**

```jsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/home" element={<Home />} />
        {/* ... más rutas */}
      </Routes>
    </BrowserRouter>
  );
}
```

### Fase 3: Desplegar en Vercel (opción recomendada)

1. Crear cuenta en https://vercel.com
2. Conectar GitHub
3. Importar repositorio IndieQuest
4. Vercel auto-detectará Vite
5. En Settings → Environment Variables:
   - `VITE_API_BASE_URL` = URL de la API (local o remota)
6. Deploy automático con `git push`
7. En Domains: añadir `indiequest.com` y actualizar DNS

### Fase 4: Actualizar client.js para variables de entorno

**Archivo: `src/api/client.js`**

Ya está configurado correctamente:
```js
const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5063/api';
```

No necesita cambios.

---

## Configuración CORS (Backend .NET)

El backend necesita permitir orígenes remotos:

**Archivo: `IndieQuest-Api/Program.cs`**

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRemote", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",           // Desarrollo local
            "https://indiequest.com",          // Producción
            "https://localhost:3000"           // Servidor local Node
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

app.UseCors("AllowRemote");
```

O más flexible (⚠️ solo en desarrollo):
```csharp
policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
```

---

## Comparativa de Opciones

| Criterio | Vercel | Netlify | Servidor Propio |
|----------|:---:|:---:|:---:|
| React Router | ✅ Automático | ✅ Automático | ✅ Manual |
| Dominio personalizado | ✅ | ✅ | ✅ |
| HTTPS automático | ✅ | ✅ | ✅ (Let's Encrypt) |
| Sin abrir puertos | ✅ | ✅ | ✅ |
| API local | ⚠️ (función proxy) | ⚠️ (función proxy) | ✅ (directo) |
| Curva aprendizaje | Baja | Baja | Alta |
| Costo | Gratuito | Gratuito | Hosting + dominio |
| Facilidad setup | 5 min | 5 min | 30 min |

**Recomendación:** **Vercel** + desplegar también la API en nube (mejor solución a largo plazo).

---

## Archivos a Crear/Modificar

1. `IndieQuest-UI/vite.config.js` → Actualizar `build.outDir`
2. `IndieQuest-UI/.env.production` → (Nuevo) URL de API para producción
3. `IndieQuest-UI/vercel.json` → (Nuevo) Config de rewrites para React Router
4. `IndieQuest-Api/Program.cs` → Actualizar CORS con dominio
5. `IndieQuest-UI/package.json` → Ya está correcto, no cambios necesarios
