# Guía: Dockerización de Indie Quest API con PostgreSQL

Esta guía explica cómo dockerizar tu proyecto Indie Quest para levantar tanto la API como la base de datos PostgreSQL usando Docker y Docker Compose.

---

## 1. ¿Qué es Docker?

**Docker** es una plataforma que permite empaquetar aplicaciones y sus dependencias en contenedores ligeros y portables. Esto facilita la ejecución consistente en cualquier entorno, evitando problemas de "funciona en mi máquina".

---

## 2. Requisitos previos

- Tener instalado [Docker Desktop](https://www.docker.com/products/docker-desktop/).
- Proyecto .NET y archivos de configuración listos.

---

## 3. Crear el Dockerfile para la API

En la carpeta `IndieQuest-Api_with_PosGre/`, crea un archivo llamado `Dockerfile` con el siguiente contenido:

```dockerfile
# Imagen base para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IQ-Api.dll"]
```

---

## 4. Crear el archivo docker-compose.yml

En la raíz de la solución (donde está el `.sln`), crea un archivo `docker-compose.yml`:

```yaml
version: '3.8'
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: Indie_Quest_DB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: jc123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build:
      context: ./IndieQuest-Api_with_PosGre
    ports:
      - "7058:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=Indie_Quest_DB;Username=postgres;Password=jc123
    depends_on:
      - db

volumes:
  postgres_data:
```

---

## 5. Ajustar la cadena de conexión

En `appsettings.json` de la API, usa el host `db`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db;Port=5432;Database=Indie_Quest_DB;Username=postgres;Password=jc123"
}
```

---

## 6. Levantar los servicios

Desde la raíz de la solución, ejecuta:

```bash
docker-compose up --build
```

Esto descargará las imágenes, construirá la API y levantará ambos servicios.

---

## 7. Notas adicionales

- Puedes acceder a la API en `http://localhost:7058` (ajusta el puerto si es necesario).
- Los datos de PostgreSQL se guardan en un volumen persistente.

---

## 8. Sugerencias de seguridad para docker-compose.yml

- **No expongas contraseñas en texto plano:** Usa variables de entorno y archivos `.env` para gestionar secretos.
- **Limita los puertos expuestos:** Solo publica los puertos necesarios para desarrollo.
- **Usa redes personalizadas:** Define una red específica para aislar los servicios.
- **No uses imágenes `latest`:** Especifica versiones concretas para evitar cambios inesperados.
- **No ejecutes contenedores como root:** Configura usuarios no privilegiados cuando sea posible.
- **No montes volúmenes sensibles en producción:** Limita el acceso a datos solo a lo necesario.
- **Elimina credenciales por defecto antes de producción:** Cambia usuario y contraseña de la base de datos.

---

> **Recuerda:** Estas prácticas ayudan a reducir riesgos de seguridad en entornos Docker.
