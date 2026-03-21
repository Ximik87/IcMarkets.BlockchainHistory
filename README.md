# IcMarkets.BlockchainHistory

dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate -p src/IcMarkets.BlockchainHistory.Infrastructure -s src/IcMarkets.BlockchainHistory.Api


dotnet ef database update   --project .\src\IcMarkets.BlockchainHistory.Infrastructure   --startup-project .\src\IcMarkets.BlockchainHistory.WorkerService\

docker compose up
docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d postgres