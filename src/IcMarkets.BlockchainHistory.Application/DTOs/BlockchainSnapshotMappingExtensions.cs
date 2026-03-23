using IcMarkets.BlockchainHistory.Domain.Entities;

namespace IcMarkets.BlockchainHistory.Application.DTOs;

public static class BlockchainSnapshotMappingExtensions
{
    public static BlockchainSnapshotResponse ToResponse(this BlockchainSnapshot snapshot)
    {
        return new BlockchainSnapshotResponse
        {
            Id = snapshot.Id,
            BlockchainType = snapshot.BlockchainType.ToString(),
            RawJson = snapshot.RawJson,
            CreatedAt = snapshot.CreatedAt,
            Height = snapshot.Height,
            Hash = snapshot.Hash,
            PeerCount = snapshot.PeerCount,
            UnconfirmedCount = snapshot.UnconfirmedCount,
            Time = snapshot.Time,
            LatestUrl = snapshot.LatestUrl,
            PreviousHash = snapshot.PreviousHash,
            PreviousUrl = snapshot.PreviousUrl,
            HighFeePerKb = snapshot.HighFeePerKb,
            MediumFeePerKb = snapshot.MediumFeePerKb,
            LowFeePerKb = snapshot.LowFeePerKb,
            LastForkHeight = snapshot.LastForkHeight,
            LastForkHash = snapshot.LastForkHash
        };
    }
}
