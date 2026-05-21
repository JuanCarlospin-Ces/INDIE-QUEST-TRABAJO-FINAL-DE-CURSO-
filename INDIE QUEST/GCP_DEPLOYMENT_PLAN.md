# Plan de despliegue a Google Cloud (Firebase + Cloud Run)

## Objetivo
Implantar la interfaz (UI) usando Firebase Hosting/Storage y la API en Cloud Run, con los archivos en Google Cloud Storage y la base de datos en Cloud SQL (PostgreSQL).

## Arquitectura propuesta
- UI: Firebase Hosting (build estático) + Firebase Storage para uploads si quieres una integración directa desde cliente.
- API: Cloud Run (containerizado, .NET 6/7) con Service Account.
- Almacenamiento de archivos: Google Cloud Storage (GCS) — o Firebase Storage (capa sobre GCS) si prefieres la UX Firebase.
- Base de datos: Cloud SQL for PostgreSQL (gestionado). Alternativa NoSQL: Firestore si reescribes acceso.
- Secretos: Secret Manager.

## Plan paso a paso
1) Preparación del proyecto
- Revisar `IndieQuest-Api` y `IndieQuest-UI` para confirmar puntos de entrada y Dockerfile.
- Añadir variables de entorno para `CONNECTION_STRING`, bucket name y credenciales.

2) Hosting UI (Firebase Hosting)
- Build de la UI (vite):

```bash
npm install
npm run build
```

- Inicializar (si hace falta) y desplegar:

```bash
firebase init hosting
firebase deploy --only hosting
```

- Si manejas uploads desde cliente, usa Firebase Storage y configura reglas de seguridad.

3) Contenerizar y desplegar API en Cloud Run
- Construir la imagen y subir a Container Registry (o Artifact Registry):

```bash
gcloud builds submit --tag gcr.io/PROJECT_ID/iq-api
```

- Desplegar en Cloud Run (añadir instancia Cloud SQL si usas Postgres):

```bash
gcloud run deploy iq-api \
  --image gcr.io/PROJECT_ID/iq-api \
  --region us-central1 \
  --platform managed \
  --add-cloudsql-instances=PROJECT:REGION:INSTANCE_NAME \
  --set-env-vars BUCKET_NAME=mi-bucket
```

- Configurar la Service Account con permisos mínimos a Cloud SQL, Storage y Secret Manager.

4) Crear Cloud SQL (Postgres) y migraciones
- Crear instancia:

```bash
gcloud sql instances create iq-db --database-version=POSTGRES_14 --cpu=1 --memory=4GB --region=us-central1
gcloud sql databases create indiequest --instance=iq-db
gcloud sql users set-password postgres --instance=iq-db --password="YOUR_PASSWORD"
```

- Para migraciones con EF Core, usar Cloud SQL Auth proxy localmente o el conector para Cloud Run; ejecutar `dotnet ef database update` desde un entorno con acceso a la instancia o desde CI.

5) Almacenamiento de archivos (GCS)
- Crear bucket y configurar CORS/lifecycle si es necesario:

```bash
gsutil mb -p PROJECT_ID -c STANDARD -l us-central1 gs://mi-bucket
gsutil cors set cors.json gs://mi-bucket
```

- Subidas recomendadas: cliente -> Storage (con signed URL o reglas) para evitar pasar archivos por la API.

6) Seguridad y secretos
- Guardar connection strings y claves en Secret Manager.
- Conceder acceso a Cloud Run Service Account para leer secretos.

7) CI/CD y automatización
- Usar Cloud Build o GitHub Actions:
  - Build imagen, ejecutar tests, ejecutar migrations, desplegar a Cloud Run y actualizar hosting si procede.

8) Backups y operativa
- Habilitar backups automáticos en Cloud SQL.
- Versionado o lifecycle en GCS para controlar costos.
- Monitorizar con Cloud Monitoring y configurar alertas.

9) Pruebas y migración
- Probar endpoints en entorno staging (servicio separado o etiquetas de Cloud Run).
- Migrar datos (dump/restore o scripts ETL) a Cloud SQL y verificar integridad.

## Puntos de integración concretos para tu repo
- `IndieQuest-Api/Program.cs` -> añadir lectura de `CONNECTION_STRING` desde Secret Manager o variable de entorno.
- `IndieQuest-LocalData` -> evaluar qué datos deben migrarse a Cloud SQL.
- Tests E2E en `IndieQuest-Tests/EndToEndTest` -> apuntarlos al entorno staging para validar despliegue.

## Recursos para profundizar
- Firebase Hosting: https://firebase.google.com/docs/hosting
- Firebase Storage: https://firebase.google.com/docs/storage
- Google Cloud Storage (GCS): https://cloud.google.com/storage/docs
- Cloud Run: https://cloud.google.com/run/docs
- Cloud SQL (Postgres): https://cloud.google.com/sql/docs/postgres
- Conectar Cloud Run a Cloud SQL: https://cloud.google.com/sql/docs/postgres/connect-run
- Secret Manager: https://cloud.google.com/secret-manager/docs
- IAM & Service Accounts: https://cloud.google.com/iam/docs
- Cloud Build (CI): https://cloud.google.com/build/docs
- Signed URLs & Uploads: https://cloud.google.com/storage/docs/access-control/signed-urls
- EF Core + Cloud SQL patterns (migraciones): https://cloud.google.com/sql/docs/postgres/connect-app-engine
- Codelabs y tutoriales prácticos: https://codelabs.developers.google.com/

## Checklist corto (acciones inmediatas)
- [ ] Crear proyecto GCP y habilitar APIs (Cloud Run, Cloud SQL, Cloud Build, Secret Manager, Cloud Storage, Firebase)
- [ ] Crear bucket GCS y configurar CORS/regras
- [ ] Crear instancia Cloud SQL y crear DB/usuario
- [ ] Construir imagen y desplegar en Cloud Run (conectar Cloud SQL)
- [ ] Desplegar UI a Firebase Hosting
- [ ] Configurar secretos y backups
- [ ] Ejecutar pruebas E2E en staging

---

Si quieres, genero ahora los comandos exactos adaptados a tu `PROJECT_ID`, nombres de instancia y ejemplos de `firebase.json` y `Dockerfile` optimizado para Cloud Run.
