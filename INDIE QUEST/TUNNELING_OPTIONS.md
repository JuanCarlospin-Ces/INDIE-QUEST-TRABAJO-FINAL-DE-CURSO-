# Opciones de tunneling para exponer tu máquina local como host

Este documento describe opciones prácticas para exponer tu aplicación ejecutándose en tu máquina local (host propio) hacia Internet o hacia otras máquinas en la red, junto con pros/cons e ideas de integración para arrancar el túnel automáticamente cuando lances la aplicación.

## Resumen rápido
- Objetivo: poder usar siempre tu propia máquina como host cuando lanzas la aplicación.
- Dos enfoques principales:
  - Exponer públicamente tu servicio (requiere puerto público/dns/seguridad).
  - Crear red privada (VPN/mesh) para acceso restringido sin abrir puertos.

## Opciones de tunneling (dev y producción)

### 1) ngrok
- Tipo: servicio SaaS + cliente local.
- Uso: `ngrok http 5000`.
- Pros: muy simple de usar, URL pública HTTPS, inspección de tráfico local, SDKs/clients para integrarlo.
- Contras: la versión gratuita rota subdominios y no es ideal para producción a gran escala; requiere registrar token.
- Integración: invocar binario `ngrok` desde la app al arrancar, o usar la API/SDK para iniciar túnel programáticamente.

### 2) Cloudflare Tunnel (`cloudflared`)
- Tipo: servicio SaaS (Cloudflare) con cliente local.
- Uso: `cloudflared tunnel run --url http://localhost:5000`.
- Pros: integrado con Cloudflare (DNS, WAF, certificados), estable para producción, sin abrir puertos en el router.
- Contras: dependencia de Cloudflare (requiere cuenta y configuración DNS).
- Integración: registrar el túnel y lanzar `cloudflared` al iniciar la app; opción de usar `service` o task scheduler.

### 3) LocalTunnel (localtunnel.me)
- Tipo: servicio comunitario + cliente.
- Pros: simple y gratuito para desarrollo.
- Contras: menos fiable, subdominios efímeros.

### 4) inlets (y inlets-pro)
- Tipo: self-hosted (cliente + servidor) o servicio pro.
- Pros: puedes auto-hospedar el servidor de túnel (control total), buena para exponer servicios en producción sin SaaS externo.
- Contras: necesitas un VPS/public server para el endpoint del túnel (pero tu tráfico pasa por ese VPS).
- Integración: arrancar el cliente `inlets` desde la app y gestionar la clave/endpoint.

### 5) FRP (Fast Reverse Proxy)
- Tipo: self-hosted reverse proxy (servidor + cliente en tu red local).
- Pros: flexible, soporte TCP/HTTP/HTTPS, ideal si controlas un servidor público.
- Contras: requiere configurar y mantener el servidor público.

### 6) SSH reverse tunnel
- Tipo: túnel SSH desde tu máquina local hacia un servidor público.
- Ejemplo: `ssh -R 8080:localhost:5000 usuario@servidor-publico`.
- Pros: sin dependencias externas adicionales, seguro si SSH está bien configurado.
- Contras: necesitas un servidor público con SSH; configuración y reconexión automática puede requerir scripts (autossh).
- Integración: usar `autossh` para mantener la conexión viva; lanzar en segundo plano al iniciar la app.

### 7) sish / serveo / similar (SSH-based)
- Tipo: proyectos que exponen túneles SSH hacia dominios públicos.
- Pros/Contras: similar a SSH reverse, convenientes para dev.

### 8) VPN / Mesh (Tailscale, ZeroTier)
- Tipo: red privada sobre Internet.
- Pros: no exponer puertos públicamente, acceso seguro entre nodos, ideal para entornos privados y pruebas.
- Contras: requiere que los clientes usen la VPN; no proporciona una URL pública por defecto.
- Integración: instalar cliente en tu máquina y en clientes remotos; opcionalmente combinar con DNS interno.

### 9) WireGuard + DNS dinámico / Port Forwarding
- Tipo: montar una VPN propia + configuración de router.
- Pros: control total, alto rendimiento.
- Contras: mayor complejidad de setup.

### 10) Dynamic DNS + Port Forwarding / UPnP
- Tipo: abrir puertos en tu router y registrar un dominio dinámico (DuckDNS, No-IP).
- Pros: tu máquina es accesible vía dominio propio.
- Contras: expone tu red; necesidad de configurar router/firewall; no funciona bien en redes restrictivas (NAT CGNAT).
- Integración: configurar script para actualizar DDNS y abrir puerto (o usar UPnP para mapear automáticamente).

## Comparativa rápida (cuándo usar cada una)
- Desarrollo rápido / compartición temporal: `ngrok`, `localtunnel`, `sish`.
- Producción sin abrir puertos: `cloudflared`, `inlets` (con servidor público propio), `FRP`.
- Privado entre máquinas (sin exponer a Internet): `Tailscale`, `ZeroTier`, `WireGuard`.
- Control total y coste bajo (requiere VPS): `FRP`, `inlets`, `SSH reverse` a tu VPS.

## Seguridad y buenas prácticas
- Siempre habilita HTTPS/TLS en el túnel cuando sea posible.
- Restringe acceso por IP o autenticación si el servicio es sensible.
- No exponer puertos administrativos (SSH/RDP) sin protección adicional.
- Rotación y gestión segura de tokens/authtokens (ngrok, cloudflared).
- Monitoriza logs del túnel y tráfico para detectar usos indebidos.

## Integración automática al lanzar tu aplicación
Sugerencias para arrancar el túnel junto con la app:

1. Iniciar proceso del cliente de túnel desde la app
- Llamar al binario del túnel (`ngrok`, `cloudflared`, `inlets`) con la configuración adecuada desde el código o un script de inicio.
- Ejemplo (bash/powershell):

```bash
# ngrok
ngrok http 5000 --log=stdout &
# cloudflared
cloudflared tunnel run --url http://localhost:5000 &
```

2. Usar librerías/SDKs
- Ngrok tiene clientes para varios lenguajes; algunos túneles ofrecen APIs para gestionar túneles desde código.

3. Reconexión automática
- Para SSH usa `autossh` para mantener y reiniciar el túnel.
- Para clientes personalizados, implementar supervisión y reinicio (systemd on Linux, Task Scheduler / NSSM on Windows, o procesos hijo que supervises desde tu app).

4. Configuración por entorno
- Dejar opciones configurables por variables de entorno: qué tipo de túnel usar, puerto, token, subdominio.
- Ejemplo: `TUNNEL_PROVIDER=ngrok`, `NGROK_AUTHTOKEN=...`, `TUNNEL_PORT=5000`.

5. Docker
- Si tu app corre en Docker, puedes incluir el cliente de túnel en otro contenedor y orquestar con docker-compose.

## Limitaciones prácticas a considerar
- Redes corporativas o ISPs con CGNAT pueden impedir abrir puertos; en esos casos los túneles salientes (ngrok, cloudflared, SSH reverse) funcionan mejor.
- Latencia y rendimiento dependen de la ruta del túnel (especialmente si el túnel pasa por un VPS o servicio externo).

## Recomendaciones finales
- Para desarrollo local y demos: usa `ngrok` o `localtunnel` por su rapidez.
- Para uso continuo en entornos controlados: `cloudflared` (si ya usas Cloudflare) o montar `inlets/FRP` con un VPS propio.
- Para acceso privado entre máquinas (sin exponer a Internet): `Tailscale` o `ZeroTier`.

---

Si quieres, puedo:
- Añadir ejemplos concretos de integración en `Program.cs` o en los scripts de inicio de tu proyecto .NET.
- Configurar el arranque automático del túnel en Windows (Task Scheduler) o Linux (systemd).
- Generar un small wrapper que lance la app y el túnel juntos según una variable `TUNNEL_PROVIDER`.

Dime qué opción(s) te interesan más y lo implemento.

## ¿La aplicación acepta conexiones externas?

Esta sección explica cómo comprobar si tu aplicación está escuchando sólo en `localhost` (127.0.0.1) o en todas las interfaces (`0.0.0.0`), y qué cambios realizar para permitir conexiones externas desde la red o Internet.

### Cómo comprobar la binding actual
- En Windows (PowerShell):

```powershell
# muestra puertos y direcciones vinculadas
netstat -ano | Select-String ":5000"
```

- Busca si la app está vinculada a `127.0.0.1:5000` (solo local) o `0.0.0.0:5000` (todas las interfaces).

### .NET (Kestrel / ASP.NET Core)
- Por defecto, en entornos de desarrollo el servidor puede estar limitado a `localhost`.
- Para aceptar conexiones externas, puedes:

1. Configurar `urls` o `ASPNETCORE_URLS`:

```powershell
# Ejecutar la app escuchando en todas las interfaces
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
dotnet run
```

2. O en `Program.cs` (mínimo ejemplo):

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");
var app = builder.Build();
app.MapGet("/", () => "Hello");
app.Run();
```

3. Si usas `appsettings.json` o configuración de Kestrel, añade el binding apropiado para escuchar en `0.0.0.0`.

### Docker
- Si tu app corre dentro de Docker, revisa el `docker run -p` o `docker-compose.yml`:

```yaml
ports:
  - "5000:5000" # mapea puerto del contenedor al host
```

- Asegúrate que dentro del contenedor la app escucha en `0.0.0.0`.

### IIS / Reverse proxy
- Si expones la app mediante IIS, Nginx o Apache, configura el reverse-proxy para aceptar conexiones y reenviarlas al puerto interno.

### Firewall y router
- En Windows, añade regla de firewall para permitir el puerto:

```powershell
# permite tráfico TCP entrante en el puerto 5000
New-NetFirewallRule -DisplayName "INDIEQUEST HTTP" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

- Si necesitas acceder desde Internet, configura port forwarding en tu router o usa una solución de túnel (ngrok, cloudflared, SSH reverse) si no quieres abrir puertos.

### Seguridad
- Si habilitas conexiones externas, aplica HTTPS/TLS y restricciones de acceso (autenticación, IP allowlist) según sensibilidad de la API.

### Resumen rápido
- Si ves `127.0.0.1` → la app solo acepta conexiones locales. Cambia `ASPNETCORE_URLS` o `UseUrls` a `0.0.0.0` y ajusta firewall/router si quieres exponerla.
- Si ves `0.0.0.0` → la app acepta conexiones externas en todas las interfaces (aún puede bloquearse por firewall/router).

Si quieres, aplico estos cambios directamente a tu `Program.cs` y creo un script PowerShell para lanzar la app + túnel automáticamente.

---

## Verificación rápida: ¿la app acepta conexiones externas?

Resumen de hallazgos sobre esta base de código y pasos prácticos para verificar y habilitar conexiones externas:

- Revisión rápida:
  - `Program.cs` no fuerza un binding; no hay `UseUrls` ni `ASPNETCORE_URLS` por defecto.
  - `Properties/launchSettings.json` está configurado para `localhost` (perfiles dev usan `http://localhost:5063` y `https://localhost:7058`).
  - `docker-compose.yml` mapea puertos `7058:8080`, pero el contenedor no establece `ASPNETCORE_URLS` ni el `Dockerfile` fuerza la app a escuchar en `0.0.0.0`.

- Conclusión corta:
  - Si ejecutas con `dotnet run` (perfil Dev): la app está ligada a `localhost` → NO acepta conexiones externas.
  - Si ejecutas con el `docker-compose` actual: es probable que tampoco acepte conexiones externas porque la app dentro del contenedor no está configurada para escuchar en `0.0.0.0`.

- Comprobaciones que puedes ejecutar ahora (PowerShell):

```powershell
# Ver puertos en uso (ajusta puerto si usas otro)
netstat -ano | Select-String ":5063"

# Obtener proceso propietario de un puerto
(Get-NetTCPConnection -LocalPort 5063).OwningProcess | Get-Process
```

- Cambios mínimos para permitir conexiones externas:
  - Opción A (variable de entorno / Docker): en `docker-compose.yml` añade para el servicio `api`:
    - `ASPNETCORE_URLS=http://0.0.0.0:8080` y confirmar el mapeo `7058:8080`.
  - Opción B (código): en `Program.cs` añadir antes de `builder.Build()`:

```csharp
builder.WebHost.UseUrls("http://0.0.0.0:5063");
```

  - Opción C (ejecución local): exportar variable y ejecutar:

```powershell
$env:ASPNETCORE_URLS = "http://0.0.0.0:5063"
dotnet run
```

- No olvides abrir el firewall y/o configurar port-forwarding en el router si quieres acceso desde otra máquina:

```powershell
New-NetFirewallRule -DisplayName "INDIEQUEST HTTP" -Direction Inbound -LocalPort 5063 -Protocol TCP -Action Allow
```

- Nota de seguridad: si expones la API, aplica HTTPS/TLS y controles de acceso (auth, IP allowlist).

Si quieres que aplique alguno de los cambios (A/B/C) ahora, indícame cuál y lo implemento y testeo en el repo.
