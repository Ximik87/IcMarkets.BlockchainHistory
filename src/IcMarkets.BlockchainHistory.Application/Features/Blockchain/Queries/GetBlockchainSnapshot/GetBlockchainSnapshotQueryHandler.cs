using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;

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
        return await _repository.GetByHashAsync(query.Hash, cancellationToken);
    }
}