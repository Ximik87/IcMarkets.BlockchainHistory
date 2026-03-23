# IcMarkets.BlockchainHistory


Install dotnet ef tool if you don't have it already
```
dotnet tool install --global dotnet-ef
```
dotnet ef migrations add InitialCreate -p src/IcMarkets.BlockchainHistory.Infrastructure -s src/IcMarkets.BlockchainHistory.Api

Update db
```
dotnet ef database update   --project .\src\IcMarkets.BlockchainHistory.Infrastructure   --startup-project .\src\IcMarkets.BlockchainHistory.WorkerService\
```

docker compose up
```
docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d postgres
```