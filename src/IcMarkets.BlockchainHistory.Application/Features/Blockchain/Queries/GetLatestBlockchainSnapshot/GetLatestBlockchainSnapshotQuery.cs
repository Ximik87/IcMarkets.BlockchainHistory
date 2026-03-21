using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;

public sealed record GetLatestBlockchainSnapshotQuery(BlockchainType BlockchainType)
    : IQuery<BlockchainSnapshotResponse?>;