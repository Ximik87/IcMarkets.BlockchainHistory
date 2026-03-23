namespace IcMarkets.BlockchainHistory.Application.DTOs;

public sealed class BlockchainSnapshotResponse
{
    public Guid Id { get; set; }
    public string BlockchainType { get; set; } = string.Empty;
    public string RawJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public long? Height { get; set; }
    public string Hash { get; set; } = string.Empty;
    public int? PeerCount { get; set; }
    public int? UnconfirmedCount { get; set; }
    public string Time { get; set; } = string.Empty;
    public string LatestUrl { get; set; } = string.Empty;
    public string PreviousHash { get; set; } = string.Empty;
    public string PreviousUrl { get; set; } = string.Empty;
    public int? HighFeePerKb { get; set; }
    public int? MediumFeePerKb { get; set; }
    public int? LowFeePerKb { get; set; }
    public int? LastForkHeight { get; set; }
    public string? LastForkHash { get; set; }
}