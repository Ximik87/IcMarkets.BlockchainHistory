using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;

public interface IBlockchainSnapshotRepository
{
    void Add(BlockchainSnapshot entity);
    void Update(BlockchainSnapshot entity);

    Task<(IReadOnlyList<BlockchainSnapshot> Items, int TotalCount)> GetHistoryAsync(
        BlockchainType blockchainType, DateTimeOffset createAt, int page, int pageSize, CancellationToken ct);

    Task<BlockchainSnapshot?> GetLatestAsync(BlockchainType blockchainType, CancellationToken ct);
    Task<BlockchainSnapshot?> GetByHashAsync(string hash, CancellationToken ct);
}