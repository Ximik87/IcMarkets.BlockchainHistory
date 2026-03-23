using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;

public sealed record GetLatestBlockchainSnapshotQuery(BlockchainType BlockchainType)
    : IQuery<BlockchainSnapshotResponse?>;

public sealed class GetLatestBlockchainSnapshotQueryHandler : IQueryHandler<GetLatestBlockchainSnapshotQuery, BlockchainSnapshotResponse?>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public GetLatestBlockchainSnapshotQueryHandler(
        IBlockchainSnapshotRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async ValueTask<BlockchainSnapshotResponse?> Handle(
        GetLatestBlockchainSnapshotQuery query,
        CancellationToken cancellationToken)
    {
        var cacheKey = GetKey(query.BlockchainType);

        if (_cache.TryGetValue(cacheKey, out BlockchainSnapshotResponse? cached))
        {
            return cached;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            // Double-check after acquiring the lock to prevent cache stampede
            if (_cache.TryGetValue(cacheKey, out BlockchainSnapshotResponse? cached2))
            {
                return cached2;
            }

            var snapshot = await _repository.GetLatestAsync(query.BlockchainType, cancellationToken);

            if (snapshot is null)
            {
                _cache.Set<BlockchainSnapshotResponse?>(cacheKey, null, CacheDuration);
                return null;
            }

            var response = new BlockchainSnapshotResponse
            {
                Id = snapshot.Id,
                BlockchainType = snapshot.BlockchainType.ToString(),
                RawJson = snapshot.RawJson,
                CreatedAt = snapshot.CreatedAt,
                Height = snapshot.Height,
                Hash = snapshot.Hash,
                PeerCount = snapshot.PeerCount,
                UnconfirmedCount = snapshot.UnconfirmedCount
            };

            _cache.Set<BlockchainSnapshotResponse?>(cacheKey, response, CacheDuration);

            return response;
        }
        finally
        {
            _lock.Release();
        }
    }

    private static string GetKey(BlockchainType blockchainType)
    {
        var type = blockchainType.ToString().Trim().ToUpperInvariant();
        return $"latest:{type}";
    }
}