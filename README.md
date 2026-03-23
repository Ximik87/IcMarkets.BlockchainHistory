# ICMarkets BlockchainHistory WebApi

Install the postgres image in Docker; the database will be accessible on port 5432.
```
docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d postgres
```

Install dotnet ef tool if not installed
```
dotnet tool install --global dotnet-ef
``` 

Update the database schema (run from the directory with solution)
```
dotnet ef database update -p .\src\IcMarkets.BlockchainHistory.Infrastructure  -s .\src\IcMarkets.BlockchainHistory.WorkerService\
```

Unfortunately, deployment via docker compose does not work at the moment, but the general scheme is described in the file docker-compose.yml

## Applications

Both applications are run locally.

### IcMarkets.BlockchainHistory.Api

ASP.NET Core Web API that exposes blockchain snapshot data through REST endpoints. In development mode it serves a Swagger UI at `http://localhost:5190/swagger`. Key endpoints:

| Endpoint | Description |
|---|---|
| `GET /api/BlockchainSnapshots/{blockchainType}` | Returns paginated history of blockchain snapshots (filtered by date, page, pageSize). |
| `GET /api/BlockchainSnapshots/{blockchainType}/latest` | Returns the most recent snapshot for a given blockchain type. |
| `GET /api/BlockchainSnapshots/types` | Lists all available blockchain types. |
| `GET /health` | Health-check (includes PostgreSQL connectivity). |

### IcMarkets.BlockchainHistory.WorkerService

Background worker service that polls external blockchain APIs every 5 minutes, fetches the latest data for every supported blockchain type, and persists snapshots to the PostgreSQL database. It also exposes a `/health` endpoint for health-checking.