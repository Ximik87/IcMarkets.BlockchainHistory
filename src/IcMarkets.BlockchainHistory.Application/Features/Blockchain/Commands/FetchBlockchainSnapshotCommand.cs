using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;

public sealed record FetchBlockchainSnapshotCommand(BlockchainType BlockchainType) : ICommand;
