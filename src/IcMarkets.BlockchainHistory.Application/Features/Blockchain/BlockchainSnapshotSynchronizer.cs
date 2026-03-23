using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetHash;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain;

public interface IBlockchainSnapshotSynchronizer
{
    Task FetchAndStoreAsync(BlockchainType blockchainType, CancellationToken ct);
}

public sealed class BlockchainSnapshotSynchronizer : IBlockchainSnapshotSynchronizer
{
    private readonly IBlockCypherClient _client;
    private readonly IMediator _mediator;

    public BlockchainSnapshotSynchronizer(
        IBlockCypherClient client,
        IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public async Task FetchAndStoreAsync(BlockchainType blockchainType, CancellationToken ct)
    {
        var data = await _client.GetAsync(blockchainType, ct);
        var rawJson = JsonSerializer.Serialize(data);
        var snapshot = BlockchainSnapshot.Create(
            blockchainType,
            rawJson,
            data.Height,
            data.Hash,
            data.PeerCount,
            data.UnconfirmedCount);

        var exist = await _mediator.Send(new GetBlockchainSnapshotQuery(snapshot.Hash), ct);

        if (exist is not null)
        {
            exist.Update(rawJson, data.Height, data.PeerCount, data.UnconfirmedCount);
            await _mediator.Send(new UpdateBlockchainSnapshotCommand(exist), ct);
        }
        else
        {
            await _mediator.Send(new SaveBlockchainSnapshotCommand(snapshot), ct);
        }
    }
}