using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;

public sealed record GetLatestBlockchainSnapshotQuery(BlockchainType BlockchainType)
    : IQuery<BlockchainSnapshotResponse?>;

public sealed class GetLatestBlockchainSnapshotQueryHandler : IQueryHandler<GetLatestBlockchainSnapshotQuery, BlockchainSnapshotResponse?>
{
    private readonly IBlockchainSnapshotRepository _repository;

    public GetLatestBlockchainSnapshotQueryHandler(IBlockchainSnapshotRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BlockchainSnapshotResponse?> Handle(
        GetLatestBlockchainSnapshotQuery query,
        CancellationToken cancellationToken)
    {
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
            UnconfirmedCount = snapshot.UnconfirmedCount
        };
    }
}