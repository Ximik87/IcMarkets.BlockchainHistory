using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.WorkerService;

public class Worker : BackgroundService
{
    private static readonly BlockchainType[] AllBlockchainTypes = Enum.GetValues<BlockchainType>();
    private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

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

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task FetchAndStoreAllAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBlockCypherClient>();
        var repository = scope.ServiceProvider.GetRequiredService<IBlockchainSnapshotRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        foreach (var blockchainType in AllBlockchainTypes)
        {
            try
            {
                var (data, sourceUrl) = await client.GetAsync(blockchainType, ct);
                var rawJson = JsonSerializer.Serialize(data);

                var snapshot = BlockchainSnapshot.Create(
                    blockchainType,
                    sourceUrl,
                    rawJson,
                    data.Height,
                    data.Hash,
                    data.PeerCount,
                    data.UnconfirmedCount);

                await repository.AddAsync(snapshot, ct);

                _logger.LogInformation(
                    "Fetched {BlockchainType}: height={Height}",
                    blockchainType,
                    data.Height);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch {BlockchainType}", blockchainType);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}