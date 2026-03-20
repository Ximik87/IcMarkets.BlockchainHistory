using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IcMarkets.BlockchainHistory.Infrastructure.Persistence;

internal sealed class BlockchainSnapshotRepository : IBlockchainSnapshotRepository
{
    private readonly AppDbContext _dbContext;

    public BlockchainSnapshotRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(BlockchainSnapshot entity, CancellationToken ct)
    {
        await _dbContext.BlockchainSnapshots.AddAsync(entity, ct);
    }

    public async Task<IReadOnlyList<BlockchainSnapshot>> GetHistoryAsync(
        BlockchainType blockchainType,
        CancellationToken ct)
    {
        return await _dbContext.BlockchainSnapshots
            .Where(s => s.BlockchainType == blockchainType)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<BlockchainSnapshot?> GetLatestAsync(
        BlockchainType blockchainType,
        CancellationToken ct)
    {
        return await _dbContext.BlockchainSnapshots
            .Where(s => s.BlockchainType == blockchainType)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }
}