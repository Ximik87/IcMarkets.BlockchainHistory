using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;

public sealed record GetBlockchainHistoryQuery(BlockchainType BlockchainType)
    : IQuery<IReadOnlyList<BlockchainSnapshotResponse>>;