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

        var response = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var snapshot = await _repository.GetLatestAsync(query.BlockchainType, cancellationToken);

            if (snapshot is null)
                return null;

            return new BlockchainSnapshotResponse
            {
                Id = snapshot.Id,
                BlockchainType = snapshot.BlockchainType.ToString(),
                RawJson = snapshot.RawJson,
                CreatedAt = snapshot.CreatedAt,
                Height = snapshot.Height,
                Hash = snapshot.Hash,
                PeerCount = snapshot.PeerCount,
                UnconfirmedCount = snapshot.UnconfirmedCount,
                Time = snapshot.Time,
                LatestUrl = snapshot.LatestUrl,
                PreviousHash = snapshot.PreviousHash,
                PreviousUrl = snapshot.PreviousUrl,
                HighFeePerKb = snapshot.HighFeePerKb,
                MediumFeePerKb = snapshot.MediumFeePerKb,
                LowFeePerKb = snapshot.LowFeePerKb,
                LastForkHeight = snapshot.LastForkHeight,
                LastForkHash = snapshot.LastForkHash
            };
        });

        return response;
    }

    private static string GetKey(BlockchainType blockchainType)
    {
        var type = blockchainType.ToString().Trim().ToUpperInvariant();
        return $"latest:{type}";
    }
}