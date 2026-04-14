using IcMarkets.BlockchainHistory.Application.Abstractions;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.WorkerService;

public sealed class BlockchainDataPollingService : BackgroundService
{
    private readonly BlockchainType[] _allBlockchainTypes = Enum.GetValues<BlockchainType>();
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(5);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BlockchainDataPollingService> _logger;

    public BlockchainDataPollingService(
        IServiceScopeFactory scopeFactory,
        ILogger<BlockchainDataPollingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker polling at: {Time}", DateTimeOffset.UtcNow);

            try
            {
                await FetchAndStoreAllAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during blockchain data polling");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }

    private async Task FetchAndStoreAllAsync(CancellationToken ct)
    {
        await Parallel.ForEachAsync(_allBlockchainTypes, ct, async (blockchainType, ctInternal) =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var coordinator = scope.ServiceProvider.GetRequiredService<IBlockchainSnapshotSynchronizer>();
                await coordinator.FetchAndStoreAsync(blockchainType, ctInternal);
                _logger.LogInformation("Fetched {BlockchainType}", blockchainType);
                // small delay because api have rate limits
                await Task.Delay(800, ctInternal);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch {BlockchainType}", blockchainType);
            }
        });
    }
}