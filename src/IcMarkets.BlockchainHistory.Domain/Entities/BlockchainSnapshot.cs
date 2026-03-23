using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Domain.Entities;

public sealed class BlockchainSnapshot
{
    public Guid Id { get; private set; }
    public BlockchainType BlockchainType { get; private set; }    
    public string RawJson { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    // optional normalized fields for quick filtering/display
    public long? Height { get; private set; }
    public string Hash { get; private set; } = string.Empty;
    public int? PeerCount { get; private set; }
    public int? UnconfirmedCount { get; private set; }

    private BlockchainSnapshot() { }

    public static BlockchainSnapshot Create(
        BlockchainType blockchainType,       
        string rawJson,
        long? height,
        string hash,
        int? peerCount,
        int? unconfirmedCount)
    {
        return new BlockchainSnapshot
        {
            Id = Guid.NewGuid(),
            BlockchainType = blockchainType,          
            RawJson = rawJson,
            CreatedAt = DateTimeOffset.UtcNow,
            Height = height,
            Hash = hash,
            PeerCount = peerCount,
            UnconfirmedCount = unconfirmedCount
        };
    }

    public void Update(      
        string rawJson,
        long? height,
        int? peerCount,
        int? unconfirmedCount)
    {     
        RawJson = rawJson;
        Height = height;
        PeerCount = peerCount;
        UnconfirmedCount = unconfirmedCount;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}