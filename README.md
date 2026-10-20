# DeviceAPI

A small REST API for keeping track of devices (for example microcontrollers like an ESP32), built with ASP.NET Core and Entity Framework Core on .NET 10.

Based on Microsoft's [Create a controller-based web API](https://learn.microsoft.com/aspnet/core/tutorials/first-web-api) tutorial.

## Features

- CRUD endpoints for devices (`Name`, `Type`, `Status`)
- DTOs (`DeviceItemDTO`) so internal fields like `Secret` are never sent to or accepted from clients
- OpenAPI document and Swagger UI in development
- Dockerfile for running the API in a container (both branches)

## Branches

| Branch | Database | Notes |
|---|---|---|
| `main` | EF Core **InMemory** | No setup needed. Data is lost every time the app restarts. |
| `postgresql` | **PostgreSQL** on [Neon](https://neon.tech) via [Npgsql](https://www.npgsql.org/efcore/) | Data is stored in a hosted database and survives restarts. |

To switch to the PostgreSQL version:

```bash
git checkout postgresql
```

## Running (main branch)

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

Then open Swagger UI at <https://localhost:7077/swagger> (or <http://localhost:5062/swagger>).

## Running (postgresql branch)

1. Create a free project on [Neon](https://neon.tech). In the **Connect** dialog, choose **.NET** to get the connection string in the format Npgsql expects (`Host=...; Database=...; Username=...; Password=...`). The `postgresql://...` URL shown by default won't work.

2. Copy the example settings file and fill in your connection string:

   ```bash
   cp appsettings.example.json appsettings.json
   ```

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=YOUR_NEON_HOST.neon.tech; Database=neondb; Username=YOUR_USERNAME; Password=YOUR_PASSWORD; SSL Mode=VerifyFull; Channel Binding=Require;"
   }
   ```

   `appsettings.json` is in `.gitignore`, so your password is never committed. Only `appsettings.example.json` (with placeholders) is in the repo.

3. Create the database tables:

   ```bash
   dotnet tool install -g dotnet-ef   # once
   dotnet ef database update
   ```

4. Run the app:

   ```bash
   dotnet run
   ```

## Running with Docker

Both branches include the same `Dockerfile` and `.dockerignore`. You only need [Docker](https://docs.docker.com/get-docker/) (or [Podman](https://podman.io)) installed, not the .NET SDK.

The `Dockerfile` builds the app with the .NET SDK image and runs it on the smaller ASP.NET runtime image. `.dockerignore` keeps `bin/`, `obj/`, `.git/` and the `appsettings` files out of the image, so no passwords end up inside it.

1. Make sure Docker is running:

   ```bash
   sudo systemctl start docker
   ```

2. Build the image (from the project folder):

   ```bash
   docker build -t deviceapi .
   ```

3. Run the container. The command depends on the branch:

### main branch (InMemory)

No database or configuration needed:

```bash
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development deviceapi
```

Data is lost when the container stops.

### postgresql branch (Neon)

Make sure the database tables exist first (step 3 of [Running (postgresql branch)](#running-postgresql-branch)). The container doesn't run migrations.

Since `appsettings.json` isn't in the image, pass the connection string as an environment variable:

```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Host=YOUR_NEON_HOST.neon.tech; Database=neondb; Username=YOUR_USERNAME; Password=YOUR_PASSWORD; SSL Mode=VerifyFull; Channel Binding=Require;" \
  deviceapi
```

The double underscore in `ConnectionStrings__DefaultConnection` maps to `ConnectionStrings:DefaultConnection` in the app's configuration.

### Using it

Open Swagger UI at <http://localhost:8080/swagger>, or call the API at <http://localhost:8080/api/DeviceItem>.

- The container listens on port **8080** (the .NET image default), not the ports from `launchSettings.json`.
- `ASPNETCORE_ENVIRONMENT=Development` turns on Swagger UI. Leave it out to run in Production mode.
- Stop the container with **Ctrl+C**, or `docker ps` then `docker stop <id>` if it runs in the background.
- Stop Docker itself with `sudo systemctl stop docker docker.socket`.
- Podman works the same way: replace `docker` with `podman` (no `systemctl` step needed).

## Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/DeviceItem` | List all devices |
| `GET` | `/api/DeviceItem/{id}` | Get one device |
| `POST` | `/api/DeviceItem` | Add a device |
| `PUT` | `/api/DeviceItem/{id}` | Update a device |
| `DELETE` | `/api/DeviceItem/{id}` | Delete a device |

Example:

```bash
curl -k -X POST https://localhost:7077/api/DeviceItem \
  -H "Content-Type: application/json" \
  -d '{"name":"ESP32","type":"Microcontroller","status":"Connected"}'
```
