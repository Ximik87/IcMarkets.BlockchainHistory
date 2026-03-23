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
    private readonly IBlockchainSnapshotRepository _repository;

    public GetBlockchainHistoryQueryHandler(
        IBlockchainSnapshotRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
    }

    public async ValueTask<IReadOnlyList<BlockchainSnapshotResponse>> Handle(GetBlockchainHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var dbTime = new DateTimeOffset(query.Time, TimeSpan.Zero);

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
            UnconfirmedCount = s.UnconfirmedCount
        }).ToList();

        return responses;
    }

    private string GetKey(BlockchainType queryBlockchainType, DateTime createdAt)
    {
        var type = queryBlockchainType.ToString().Trim().ToUpperInvariant();
        var utc = createdAt.ToUniversalTime();

        return $"history:{type}:{utc:O}";
    }
}