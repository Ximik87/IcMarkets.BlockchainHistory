# IcMarkets.BlockchainHistory

dotnet ef migrations add InitialCreate -p src/IcMarkets.BlockchainHistory.Infrastructure -s src/IcMarkets.BlockchainHistory.Api


dotnet ef database update   --project .\src\IcMarkets.BlockchainHistory.Infrastructure   --startup-project .\src\IcMarkets.BlockchainHistory.WorkerService\

docker compose up