using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;

public sealed record GetBlockchainHistoryQuery(BlockchainType BlockchainType, DateTime Time, int Page = 1, int PageSize = 50)
    : IQuery<PagedResponse<BlockchainSnapshotResponse>>;

public sealed class GetBlockchainHistoryQueryHandler
    : IQueryHandler<GetBlockchainHistoryQuery, PagedResponse<BlockchainSnapshotResponse>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IMemoryCache _cache;

    public GetBlockchainHistoryQueryHandler(
        IBlockchainSnapshotRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async ValueTask<PagedResponse<BlockchainSnapshotResponse>> Handle(GetBlockchainHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var cacheKey = GetKey(query.BlockchainType, query.Time, query.Page, query.PageSize);
        var dbTime = new DateTimeOffset(query.Time, TimeSpan.Zero);

        var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var (snapshots, totalCount) = await _repository.GetHistoryAsync(
                query.BlockchainType, dbTime, query.Page, query.PageSize, cancellationToken);

            var responses = snapshots.Select(s => s.ToResponse()).ToList();

            return new PagedResponse<BlockchainSnapshotResponse>(responses, query.Page, query.PageSize, totalCount);
        });

        return result!;
    }

    private static string GetKey(BlockchainType blockchainType, DateTime createdAt, int page, int pageSize)
    {
        var type = blockchainType.ToString().Trim().ToUpperInvariant();
        var utc = createdAt.ToUniversalTime();

        return $"history:{type}:{utc:O}:p{page}:s{pageSize}";
    }
}