# DeviceAPI

A small REST API for keeping track of devices (for example microcontrollers like an ESP32), built with ASP.NET Core and Entity Framework Core on .NET 10.

Based on Microsoft's [Create a controller-based web API](https://learn.microsoft.com/aspnet/core/tutorials/first-web-api) tutorial.

## Features

- CRUD endpoints for devices (`Name`, `Type`, `Status`)
- DTOs (`DeviceItemDTO`) so internal fields like `Secret` are never sent to or accepted from clients
- OpenAPI document and Swagger UI in development

## Branches

| Branch | Database | Notes |
|---|---|---|
| `main` | EF Core **InMemory** | No setup needed. Data is lost every time the app restarts. |
| `postgresql` | **PostgreSQL** via [Npgsql](https://www.npgsql.org/efcore/) | Data is stored in a real database and survives restarts. |

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

1. Have a PostgreSQL server running. The quickest way is Docker:

   ```bash
   docker run --name deviceapi-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
   ```

2. Set the connection string in `appsettings.Development.json`:

   ```json
   "ConnectionStrings": {
     "DeviceContext": "Host=localhost;Port=5432;Database=deviceapi;Username=postgres;Password=postgres"
   }
   ```

3. Create the database tables:

   ```bash
   dotnet tool install -g dotnet-ef   # once
   dotnet ef database update
   ```

4. Run the app:

   ```bash
   dotnet run
   ```

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
