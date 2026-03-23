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
}