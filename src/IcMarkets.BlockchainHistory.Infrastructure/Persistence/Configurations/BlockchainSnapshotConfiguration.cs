using IcMarkets.BlockchainHistory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IcMarkets.BlockchainHistory.Infrastructure.Persistence.Configurations;

internal sealed class BlockchainSnapshotConfiguration : IEntityTypeConfiguration<BlockchainSnapshot>
{
    public void Configure(EntityTypeBuilder<BlockchainSnapshot> builder)
    {
        builder.ToTable("blockchain_snapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.BlockchainType)
            .HasColumnName("blockchain_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();    

        builder.Property(x => x.RawJson)
            .HasColumnName("raw_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.Height)
            .HasColumnName("height");

        builder.Property(x => x.Hash)
            .HasColumnName("hash")
            .HasMaxLength(256);

        builder.Property(x => x.PeerCount)
            .HasColumnName("peer_count");

        builder.Property(x => x.UnconfirmedCount)
            .HasColumnName("unconfirmed_count");

        builder.Property(x => x.Time)
            .HasColumnName("time")
            .HasMaxLength(256);

        builder.Property(x => x.LatestUrl)
            .HasColumnName("latest_url")
            .HasMaxLength(512);

        builder.Property(x => x.PreviousHash)
            .HasColumnName("previous_hash")
            .HasMaxLength(256);

        builder.Property(x => x.PreviousUrl)
            .HasColumnName("previous_url")
            .HasMaxLength(512);

        builder.Property(x => x.HighFeePerKb)
            .HasColumnName("high_fee_per_kb");

        builder.Property(x => x.MediumFeePerKb)
            .HasColumnName("medium_fee_per_kb");

        builder.Property(x => x.LowFeePerKb)
            .HasColumnName("low_fee_per_kb");

        builder.Property(x => x.LastForkHeight)
            .HasColumnName("last_fork_height");

        builder.Property(x => x.LastForkHash)
            .HasColumnName("last_fork_hash")
            .HasMaxLength(256);

        builder.HasIndex(x => x.Hash)
            .HasDatabaseName("ix_blockchain_snapshots_hash")
            .IsUnique()
            .HasFilter("hash IS NOT NULL");

        builder.HasIndex(x => new { x.BlockchainType, x.CreatedAt })
            .HasDatabaseName("ix_blockchain_snapshots_type_created");
    }
}
