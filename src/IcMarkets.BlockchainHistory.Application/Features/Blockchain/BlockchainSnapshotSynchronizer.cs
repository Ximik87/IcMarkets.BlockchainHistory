using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain;

public sealed class BlockchainSnapshotSynchronizer : IBlockchainSnapshotSynchronizer
{
    private readonly IBlockCypherClient _client;
    private readonly IMediator _mediator;
    private readonly TimeProvider _timeProvider; 

    public BlockchainSnapshotSynchronizer(
        IBlockCypherClient client,
        IMediator mediator, 
        TimeProvider timeProvider)
    {
        _client = client;
        _mediator = mediator;
        _timeProvider = timeProvider;
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
            data.UnconfirmedCount,
            _timeProvider.GetUtcNow(),
            data.Time,
            data.LatestUrl,
            data.PreviousHash,
            data.PreviousUrl,
            data.HighFeePerKb,
            data.MediumFeePerKb,
            data.LowFeePerKb,
            data.LastForkHeight,
            data.LastForkHash);

        var exist = await _mediator.Send(new GetBlockchainSnapshotQuery(snapshot.Hash), ct);

        if (exist is not null)
        {
            exist.Update(
                rawJson,
                data.Height,
                data.PeerCount,
                data.UnconfirmedCount,
                _timeProvider.GetUtcNow(),
                data.Time,
                data.LatestUrl,
                data.PreviousHash,
                data.PreviousUrl,
                data.HighFeePerKb,
                data.MediumFeePerKb,
                data.LowFeePerKb,
                data.LastForkHeight,
                data.LastForkHash);
            await _mediator.Send(new UpdateBlockchainSnapshotCommand(exist), ct);
        }
        else
        {
            await _mediator.Send(new SaveBlockchainSnapshotCommand(snapshot), ct);
        }
    }
}