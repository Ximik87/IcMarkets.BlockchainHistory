using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Infrastructure.Persistence;

internal sealed class BlockchainSnapshotRepository : IBlockchainSnapshotRepository
{
    public Task AddAsync(BlockchainSnapshot entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<BlockchainSnapshot>> GetHistoryAsync(BlockchainType blockchainType, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<BlockchainSnapshot?> GetLatestAsync(BlockchainType blockchainType, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}