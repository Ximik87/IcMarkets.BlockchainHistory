using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.WorkerService;

public class Worker : BackgroundService
{
    private readonly BlockchainType[] _allBlockchainTypes = Enum.GetValues<BlockchainType>();
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger)
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
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        foreach (var blockchainType in _allBlockchainTypes)
        {
            try
            {
                await mediator.Send(new FetchBlockchainSnapshotCommand(blockchainType), ct);

                _logger.LogInformation("Fetched {BlockchainType}", blockchainType);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch {BlockchainType}", blockchainType);
            }
        }
    }
}