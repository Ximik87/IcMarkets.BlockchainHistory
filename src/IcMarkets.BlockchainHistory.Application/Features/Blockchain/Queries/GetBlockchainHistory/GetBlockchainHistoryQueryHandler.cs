using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;

public sealed record GetBlockchainHistoryQuery(BlockchainType BlockchainType, DateTime Time)
    : IQuery<IReadOnlyList<BlockchainSnapshotResponse>>;

public sealed class GetBlockchainHistoryQueryHandler
    : IQueryHandler<GetBlockchainHistoryQuery, IReadOnlyList<BlockchainSnapshotResponse>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public GetBlockchainHistoryQueryHandler(
        IBlockchainSnapshotRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async ValueTask<IReadOnlyList<BlockchainSnapshotResponse>> Handle(GetBlockchainHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var cacheKey = GetKey(query.BlockchainType, query.Time);
        var dbTime = new DateTimeOffset(query.Time, TimeSpan.Zero);

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<BlockchainSnapshotResponse>? cached)
            && cached is not null)
        {
            return cached;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            // Double-check after acquiring the lock to prevent cache stampede
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<BlockchainSnapshotResponse>? cached2)
                && cached2 is not null)
            {
                return cached2;
            }

            var snapshots = await _repository.GetHistoryAsync(query.BlockchainType, dbTime, cancellationToken);

            var responses = snapshots.Select(s => new BlockchainSnapshotResponse
            {
                Id = s.Id,
                BlockchainType = s.BlockchainType.ToString(),
                RawJson = s.RawJson,
                CreatedAt = s.CreatedAt,
                Height = s.Height,
                Hash = s.Hash,
                PeerCount = s.PeerCount,
                UnconfirmedCount = s.UnconfirmedCount,
                Time = s.Time,
                LatestUrl = s.LatestUrl,
                PreviousHash = s.PreviousHash,
                PreviousUrl = s.PreviousUrl,
                HighFeePerKb = s.HighFeePerKb,
                MediumFeePerKb = s.MediumFeePerKb,
                LowFeePerKb = s.LowFeePerKb,
                LastForkHeight = s.LastForkHeight,
                LastForkHash = s.LastForkHash
            }).ToList();

            _cache.Set<IReadOnlyList<BlockchainSnapshotResponse>>(cacheKey, responses, CacheDuration);

            return responses;
        }
        finally
        {
            _lock.Release();
        }
    }

    private static string GetKey(BlockchainType blockchainType, DateTime createdAt)
    {
        var type = blockchainType.ToString().Trim().ToUpperInvariant();
        var utc = createdAt.ToUniversalTime();

        return $"history:{type}:{utc:O}";
    }
}