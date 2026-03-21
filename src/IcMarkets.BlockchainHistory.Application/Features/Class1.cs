using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Application.Features;

public sealed record FetchBlockchainSnapshotCommand(BlockchainType BlockchainType);
public sealed record GetBlockchainHistoryQuery(BlockchainType BlockchainType);

internal sealed class Class1
{
}