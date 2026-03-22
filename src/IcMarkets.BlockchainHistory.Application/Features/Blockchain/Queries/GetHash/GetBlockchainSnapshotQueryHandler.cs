using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetHash;

public sealed record GetBlockchainSnapshotQuery(string Hash)
    : IQuery<BlockchainSnapshotResponse?>;

public sealed class
    GetBlockchainSnapshotQueryHandler : IQueryHandler<GetBlockchainSnapshotQuery, BlockchainSnapshotResponse?>
{
    private readonly IBlockchainSnapshotRepository _repository;

    public GetBlockchainSnapshotQueryHandler(IBlockchainSnapshotRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BlockchainSnapshotResponse?> Handle(GetBlockchainSnapshotQuery query,
        CancellationToken cancellationToken)
    {
        var snapshot = await _repository.GetByHashAsync(query.Hash, cancellationToken);
        if (snapshot is null)
            return null;

        return new BlockchainSnapshotResponse
        {
            Id = snapshot.Id,
            RawJson = snapshot.RawJson,
            BlockchainType = snapshot.BlockchainType.ToString(),
            Hash = snapshot.Hash,
            Height = snapshot.Height,
            PeerCount = snapshot.PeerCount,
            UnconfirmedCount = snapshot.UnconfirmedCount,
            CreatedAt = snapshot.CreatedAt
        };
    }
}