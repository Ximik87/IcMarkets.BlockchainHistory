using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetHash;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain;

public interface ICoordinator
{
    Task FetchAndStoreAsync(BlockchainType blockchainType, CancellationToken cancellationToken);
}

public sealed class Coordinator : ICoordinator
{
    private readonly IBlockCypherClient _client;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;

    public Coordinator(
        IBlockCypherClient client,
        IServiceScopeFactory serviceScopeFactory,
        IMediator mediator)
    {
        _client = client;
        _serviceScopeFactory = serviceScopeFactory;
        _mediator = mediator;
    }

    public async Task FetchAndStoreAsync(BlockchainType blockchainType, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var data = await _client.GetAsync(blockchainType, cancellationToken);
        var rawJson = JsonSerializer.Serialize(data);
        var snapshot = BlockchainSnapshot.Create(
            blockchainType,
            rawJson,
            data.Height,
            data.Hash,
            data.PeerCount,
            data.UnconfirmedCount);

        var exist = await _mediator.Send(new GetBlockchainSnapshotQuery(data.Hash), cancellationToken);

        if (exist is not null)
        {
            await _mediator.Send(new UpdateBlockchainSnapshotCommand(snapshot), cancellationToken);
        }
        else
        {
            await _mediator.Send(new SaveBlockchainSnapshotCommand(snapshot), cancellationToken);
        }
    }
}