using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features;

public sealed record FetchCommand(BlockchainType BlockchainType) : ICommand;

public sealed record GetBlockchainQuery(BlockchainType BlockchainType) : IQuery<List<string>>;

public sealed class CommandHandler : ICommandHandler<FetchCommand>
{
    public ValueTask<Unit> Handle(FetchCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Fetching data for {command.BlockchainType}");
        return ValueTask.FromResult(Unit.Value);
    }
}

public sealed class QueryHandler : IQueryHandler<GetBlockchainQuery, List<string>>
{
    public ValueTask<List<string>> Handle(GetBlockchainQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Getting data for {query.BlockchainType}");
        return ValueTask.FromResult(new List<string> { "Data1", "Data2", "Data3" });
    }
}