# Quickstart: Photo Album Organizer

## Prerequisites

- Latest stable Angular CLI environment
- Latest stable .NET SDK for ASP.NET Core
- SQL Server Express `SQLEXPRESS` instance installed and running on the PC (or another SQL Server instance configured below)
- Windows authentication enabled for SQL Server access
- A directory on the local workstation where original photo files are stored

## Setup

1. Confirm SQL Server Express is running: `Get-Service 'MSSQL$SQLEXPRESS'`.
2. Ensure the Windows account running the app has permission to create a database on that instance. On the first Development API startup, the app creates the `PhotoOrganizer` database and schema if they do not already exist. For an existing database or non-Development deployment, create the database and apply `backend/database/schema/001_init.sql` manually.
3. Configure `ConnectionStrings:DefaultConnection` and `PhotoStorage:RootPath` in `backend/src/PhotoOrganizer.Api/appsettings.Development.json`. The default connection uses `.\SQLEXPRESS`; change the server name here if using a different SQL Server instance.
4. Keep the photo library directory local. The importer scans supported image files in that directory and stores only metadata in SQL Server.

## Run the backend

```bash
dotnet restore backend/PhotoOrganizer.sln
dotnet run --project backend/src/PhotoOrganizer.Api
```

The development API listens on `http://localhost:5161`. It uses the configured SQL Server instance with Windows authentication; it does not depend on the LocalDB automatic-instance runtime.

## Run the frontend

```bash
cd frontend
npm install
npm start
```

Expected outcome: the Angular application loads the album overview at `http://localhost:4200`. Its development proxy forwards `/api` requests to the local API.

Use **Import local photos** to scan the configured library folder. The importer reads EXIF capture dates when available; files without usable dates remain visible in the **Unspecified Dates** group. Original images are served from their local paths and are not uploaded or copied.

Create or rename albums from the overview. Date-grouped albums are refreshed when the local import is run.

## Validation scenarios

### Scenario 1: Browse by date

- Import photos from multiple dates.
- Open the organizer home view.
- Confirm that albums are grouped chronologically.

Expected: date-based grouping is clear and users can quickly locate photo collections by time.

### Scenario 2: Preview tiles

- Create at least two albums with representative cover photos.
- Open the overview.
- Confirm each album tile shows a preview image, album title, date range, and photo count.

Expected: the album overview is scannable without opening each album.

### Scenario 3: Resolve missing dates

- Add a photo without a valid date in metadata.
- Run **Import local photos** and confirm the photo appears in the fallback group.

Expected: the library remains usable even when date metadata is incomplete.

### Scenario 4: Protect metadata integrity

- Add and rename albums and verify metadata updates persist in SQL Server.
- Confirm the source image files remain local and unchanged.

Expected: metadata remains consistent, and images stay on disk rather than being copied to a remote store.

## Quality checks

```bash
dotnet format backend/PhotoOrganizer.sln --verify-no-changes
dotnet test backend/PhotoOrganizer.sln
cd frontend
npm run lint
npm test
npm run build
```
