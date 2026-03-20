namespace IcMarkets.BlockchainHistory.Application.DTOs;

public sealed class BlockchainSnapshotResponse
{
    public Guid Id { get; set; }
    public string BlockchainType { get; set; } = default!;
    public string SourceUrl { get; set; } = default!;
    public string RawJson { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public long? Height { get; set; }
    public string? Hash { get; set; }
    public int? PeerCount { get; set; }
    public int? UnconfirmedCount { get; set; }
}
