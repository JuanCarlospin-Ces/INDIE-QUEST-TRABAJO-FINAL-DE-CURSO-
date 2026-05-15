# GuíaCompletadeDespliegue - IndieQuest con Dominio Personalizado

## 📋 Resumen

Tu aplicación React estará disponible en **`indiequest.vercel.app`** con tu dominio personalizado **`indiequest.com`** (opcional), mientras que la API `.NET` permanece local en tu ordenador.

---

## 🚀 Flujo Completo

### Fase 1: Desarrollo Local (sin cambios requeridos)

```bash
npm run dev
```

- Frontend: `http://localhost:5173`
- API: `http://localhost:5063/api`
- React Router funciona automáticamente
- Los cambios se reflejan en tiempo real

**Archivo de configuración:** `.env` (ya creado)

---

### Fase 2: Preparar API para Producción

La API está en tu máquina local (`localhost:5063`). Para que sea accesible desde Vercel, necesitas exponerla con un túnel.

#### Opción A: Localtunnel (Recomendado - Gratis)

**1. Abre una terminal NUEVA en el directorio raíz del proyecto**

**2. Ejecuta el comando:**
```bash
npm run tunnel:api
```

**3. Verás algo como:**
```
your url is: https://indiequest-api-xxxx.loca.lt
```

**⚠️ IMPORTANTE:** Este comando debe mantenerse ejecutando mientras Vercel accede a tu API.

**4. Copia la URL completa**

---

### Fase 3: Actualizar Variables de Entorno

**1. Abre `IndieQuest-UI/.env.production`**

**2. Reemplaza:**
```env
VITE_API_BASE_URL=https://indiequest-api-xxxx.loca.lt/api
```

Con la URL real que obtuviste en Fase 2, por ejemplo:
```env
VITE_API_BASE_URL=https://indiequest-api-a1b2c.loca.lt/api
```

**3. Guarda el archivo**

---

### Fase 4: Construir para Producción

```bash
npm run build
```

Genera la carpeta `dist/` lista para Vercel.

---

### Fase 5: Desplegar en Vercel

#### Opción A: Con Git (Recomendado)

**1. Sube los cambios a GitHub:**
```bash
git add .
git commit -m "Add deployment configuration for IndieQuest with local API"
git push origin main
```

**2. Ve a https://vercel.com/new**

**3. Importa el repositorio GitHub**

**4. Vercel auto-detectará:**
- Framework: Vite
- Build Command: `npm run build`
- Output Directory: `dist`

**5. En "Environment Variables", añade:**
```
VITE_API_BASE_URL = https://indiequest-api-xxxx.loca.lt/api
```

**6. Click en "Deploy"**

✅ Tu app estará disponible en `https://indiequest.vercel.app` en ~2 minutos

---

#### Opción B: Deploy Manual (sin Git)

**1. Instala Vercel CLI:**
```bash
npm i -g vercel
```

**2. Autentica:**
```bash
vercel login
```

**3. Deploy:**
```bash
vercel --prod
```

**4. Cuando pregunte por variables de entorno, añade:**
```
VITE_API_BASE_URL = https://indiequest-api-xxxx.loca.lt/api
```

---

### Fase 6: Añadir Dominio Personalizado (Opcional)

Para usar `indiequest.com` en lugar de `indiequest.vercel.app`:

**1. Compra el dominio en:**
- GoDaddy
- Namecheap
- Google Domains
- O tu registrador favorito

**2. En Vercel Dashboard → Settings → Domains**

**3. Añade `indiequest.com` y sigue las instrucciones de DNS**

**4. HTTPS se genera automáticamente (Let's Encrypt)**

---

## ✅ Checklist de Validación

Después del deploy, verifica:

- [ ] `https://indiequest.vercel.app` carga la página
- [ ] Las rutas de React Router funcionan (`/home`, `/profile`, etc.) sin errores 404
- [ ] Las llamadas a API funcionan (comprueba Network tab en DevTools)
- [ ] Los datos se cargan correctamente desde la API local

**Para verificar Network:**
1. Abre DevTools (`F12`)
2. Ve a la pestaña "Network"
3. Haz una acción que llamea la API
4. Debería ver una petición a `https://indiequest-api-xxxx.loca.lt/api/...`

---

## 🔧 Troubleshooting

### "React Router devuelve 404 en rutas"

**Solución:** El `vercel.json` debe tener:
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

✅ Ya está configurado.

---

### "API devuelve CORS error"

**Solución 1:** Verifica que tu máquina local está corriendo `npm run tunnel:api`

**Solución 2:** En `Program.cs`, la política CORS ya permite orígenes remotos:
```csharp
policy.AllowAnyOrigin()
```

---

### "API Timeout o no responde"

**Causas comunes:**
1. El comando `npm run tunnel:api` se cerró → Reinicia en terminal separada
2. La API local (.NET) no está corriendo → `dotnet run` en `IndieQuest-Api/`
3. URL en `.env.production` es incorrecta → Verifica exactamente lo que dice `localtunnel`

---

## 📊 Variables de Entorno Resumidas

| Archivo | Variable | Valor Desarrollo | Valor Producción |
|---------|----------|-----------------|-----------------|
| `.env` | `VITE_API_BASE_URL` | `http://localhost:5063/api` | N/A (no usado) |
| `.env.production` | `VITE_API_BASE_URL` | N/A | `https://indiequest-api-xxxx.loca.lt/api` |
| Vercel Dashboard | `VITE_API_BASE_URL` | N/A | `https://indiequest-api-xxxx.loca.lt/api` |

---

## 🔄 Flujo Posterior a Deploy

### Cuando cambies código del frontend:

```bash
npm run build
git push origin main
```

Vercel redeploy automáticamente en ~1 minuto.

### Cuando cambies la API:

1. `dotnet run` en `IndieQuest-Api/`
2. No requiere redeployar Vercel
3. Los cambios están disponibles inmediatamente

---

## 🛡️ Notas de Seguridad

⚠️ **En desarrollo:** CORS permite todos los orígenes (`AllowAnyOrigin()`)

📝 **Para producción:** Actualiza `Program.cs`:
```csharp
policy.WithOrigins(
    "https://indiequest.vercel.app",
    "https://indiequest.com",
    "https://indiequest-api-xxxx.loca.lt"
)
.AllowAnyMethod()
.AllowAnyHeader();
```

---

## ❓ ¿Preguntas Frecuentes?

**P: ¿Necesito instalar Node.js en el servidor?**
A: No, Vercel maneja todo. Solo necesitas Node.js en tu máquina local.

**P: ¿Qué pasa si se corta la conexión a `localtunnel`?**
A: Vercel seguirá sirviendo la interfaz, pero fallará al intentar acceder a datos de la API. Reinicia `npm run tunnel:api`.

**P: ¿Puedo desplegar la API también?**
A: Sí, en Azure, AWS, Heroku, etc. En ese caso, cambias `VITE_API_BASE_URL` a la URL pública de la API desplegada.

---

## 📞 Soporte

- **Vercel:** https://vercel.com/docs
- **Vite:** https://vitejs.dev/guide/ssr.html
- **React Router:** https://reactrouter.com/
