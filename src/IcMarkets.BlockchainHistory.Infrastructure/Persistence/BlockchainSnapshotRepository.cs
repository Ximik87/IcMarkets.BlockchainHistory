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

    public void Add(BlockchainSnapshot entity)
    {
        _dbContext.BlockchainSnapshots.Add(entity);
    }

    public void Update(BlockchainSnapshot entity)
    {
        _dbContext.BlockchainSnapshots.Update(entity);
    }

    public async Task<IReadOnlyList<BlockchainSnapshot>> GetHistoryAsync(
        BlockchainType blockchainType,
        DateTimeOffset createAt,
        CancellationToken ct)
    {
        return await _dbContext.BlockchainSnapshots
            .Where(s => s.BlockchainType == blockchainType && s.CreatedAt >= createAt)
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

    public async Task<BlockchainSnapshot?> GetByHashAsync(string hash, CancellationToken ct)
    {
        return await _dbContext.BlockchainSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Hash == hash, ct);
    }
}