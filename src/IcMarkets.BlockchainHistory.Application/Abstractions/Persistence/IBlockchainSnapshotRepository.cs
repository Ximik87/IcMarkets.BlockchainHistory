using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;

public interface IBlockchainSnapshotRepository
{
    Task AddAsync(BlockchainSnapshot entity, CancellationToken ct);
    Task<IReadOnlyList<BlockchainSnapshot>> GetHistoryAsync(
        BlockchainType blockchainType,
        CancellationToken ct);

    Task<BlockchainSnapshot?> GetLatestAsync(
        BlockchainType blockchainType,
        CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}

public interface IBlockCypherClient
{

}