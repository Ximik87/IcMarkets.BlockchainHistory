using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;

public sealed record GetBlockchainHistoryQuery(BlockchainType BlockchainType)
    : IQuery<IReadOnlyList<BlockchainSnapshotResponse>>;

public sealed class GetBlockchainHistoryQueryHandler
    : IQueryHandler<GetBlockchainHistoryQuery, IReadOnlyList<BlockchainSnapshotResponse>>
{
    private readonly IBlockchainSnapshotRepository _repository;

    public GetBlockchainHistoryQueryHandler(IBlockchainSnapshotRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IReadOnlyList<BlockchainSnapshotResponse>> Handle(
        GetBlockchainHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var snapshots = await _repository.GetHistoryAsync(query.BlockchainType, cancellationToken);

        return snapshots.Select(s => new BlockchainSnapshotResponse
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
    }
}