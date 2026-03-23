using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Application.Abstractions;

public interface IBlockchainSnapshotSynchronizer
{
    Task FetchAndStoreAsync(BlockchainType blockchainType, CancellationToken ct);
}