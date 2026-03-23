# IÑMarkets BlockchainHistory WebApi

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