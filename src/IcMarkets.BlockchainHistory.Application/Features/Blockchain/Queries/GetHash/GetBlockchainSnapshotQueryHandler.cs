using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetHash;

public sealed record GetBlockchainSnapshotQuery(string Hash)
    : IQuery<BlockchainSnapshot?>;

public sealed class GetBlockchainSnapshotQueryHandler : IQueryHandler<GetBlockchainSnapshotQuery, BlockchainSnapshot?>
{
    private readonly IBlockchainSnapshotRepository _repository;

    public GetBlockchainSnapshotQueryHandler(IBlockchainSnapshotRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BlockchainSnapshot?> Handle(GetBlockchainSnapshotQuery query,
        CancellationToken cancellationToken)
    {
        var snapshot = await _repository.GetByHashAsync(query.Hash, cancellationToken);
        if (snapshot is null)
            return null;

        return BlockchainSnapshot.LoadFromDb(
            snapshot.Id,
            snapshot.BlockchainType,
            snapshot.RawJson,
            snapshot.CreatedAt,
            snapshot.Height,
            snapshot.Hash,
            snapshot.PeerCount,
            snapshot.UnconfirmedCount
        );
    }
}