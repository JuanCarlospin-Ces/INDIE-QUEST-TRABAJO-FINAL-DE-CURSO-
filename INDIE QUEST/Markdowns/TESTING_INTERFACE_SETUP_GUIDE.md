# IndieQuest Testing Interface — Setup Guide

A small React + React Router web client used to manually test the IndieQuest
API endpoints. It looks like a stripped-down Twitter / Bluesky timeline.

---

## 1. Prerequisites

Install once on your machine:

| Tool | Version | Notes |
|------|---------|-------|
| [Node.js](https://nodejs.org/) | **18 LTS or newer** (20 recommended) | `node -v` to check |
| npm | comes with Node | `npm -v` to check |
| .NET SDK | already used by the API | needed to run the backend |

> ✅ The interface only needs Node.js. The API must be running separately so
> the UI has data to fetch.

---

## 2. Start the IndieQuest API first

The frontend talks to the API at **`http://localhost:5063/api`** by default
(this matches `IndieQuest-Api/Properties/launchSettings.json`).

In a terminal at the repo root:

```powershell
cd "IndieQuest-Api"
dotnet run
```

Leave that terminal open. You should see something like:

```
Now listening on: http://localhost:5063
```

> If you prefer the PostgreSQL flavor, run `IndieQuest-Api_with_PosGre`
> instead. Make sure the resulting URL still matches the one configured
> in step 4 (or override it via `.env`).

---

## 3. Install the frontend dependencies

Open a **second** terminal:

```powershell
cd "IndieQuest-TestingInterface"
npm install
```

This installs:

- `react`, `react-dom`
- `react-router-dom` (routing)
- `vite` + `@vitejs/plugin-react` (dev server / build tool)

---

## 4. (Optional) Configure the API URL

The default base URL is `http://localhost:5063/api`. To change it, copy the
example env file and edit it:

```powershell
Copy-Item .env.example .env
notepad .env
```

```env
VITE_API_BASE_URL=http://localhost:5063/api
```

Restart `npm run dev` after editing `.env`.

---

## 5. Run the dev server

```powershell
npm run dev
```

Vite will print:

```
  ➜  Local:   http://localhost:5173/
```

The browser should open automatically. If not, visit
[http://localhost:5173](http://localhost:5173).

---

## 6. Available scripts

| Command          | What it does                                  |
|------------------|-----------------------------------------------|
| `npm run dev`    | Start the Vite dev server with hot reload    |
| `npm run build`  | Produce a production build in `dist/`         |
| `npm run preview`| Serve the production build locally            |

---

## 7. App overview

| Route             | Description                                     |
|-------------------|-------------------------------------------------|
| `/feed`           | Timeline of all posts (sorted by date)          |
| `/users`          | List of registered users                        |
| `/users/:id`      | User profile + their posts                      |
| `/posts/:id`      | Single post detail view                         |
| `/compose`        | Create a new post                               |
| `/register`       | Register a new user                             |

API endpoints used:

- `GET /api/Post`, `GET /api/Post/{id}`, `GET /api/Post/user/{userId}`
- `POST /api/Post`, `PUT /api/Post/{id}`, `DELETE /api/Post/{id}`
- `GET /api/User`, `GET /api/User/{id}`
- `POST /api/User`, `PUT /api/User/{id}`, `DELETE /api/User/{id}`

---

## 8. Media handling (mockups)

Real media uploads are not implemented in the API yet, so the UI uses
placeholders:

- **User avatars** → generic SVG avatar with the user's first initial.
- **Post media** → if `mediaContent` looks like an image/video URL, it is
  rendered inline. Otherwise the card shows a striped fallback box that
  reads **"MEDIA CANNOT BE LOADED"**.
- Broken image URLs are replaced at runtime with the same fallback box.

When the backend starts returning real upload URLs / file paths, no UI
changes will be needed — the existing detection logic in
[src/utils/media.js](IndieQuest-TestingInterface/src/utils/media.js) will
pick them up.

---

## 9. CORS

The API already enables CORS for any origin (`AllowAll` policy in
`Program.cs`), so no extra setup is required.

If you ever lock that down, allow at least:

```
http://localhost:5173
```

---

## 10. Troubleshooting

| Symptom | Fix |
|---------|-----|
| `Failed to fetch` / `API 0` | The API isn't running — start `IndieQuest-Api` (step 2). |
| `API 404` / empty feed | Database has no data. Use **Register** then **Compose** to seed it, or run the SQL in `IndieQuest-DataBase/`. |
| Wrong port | Edit `.env` (`VITE_API_BASE_URL`) and restart `npm run dev`. |
| Port 5173 already in use | Stop the other process or change the port in `vite.config.js`. |
| `npm install` SSL/proxy errors | Configure your corporate proxy (`npm config set proxy ...`) and retry. |

---

Happy testing! 🚀
