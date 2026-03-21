using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IcMarkets.BlockchainHistory.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<BlockchainSnapshot> BlockchainSnapshots { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
        => await base.SaveChangesAsync(ct);
}