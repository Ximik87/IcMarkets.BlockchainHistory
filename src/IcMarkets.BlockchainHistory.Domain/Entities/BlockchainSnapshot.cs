using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Domain.Entities;

public sealed class BlockchainSnapshot
{
    public Guid Id { get; private set; }
    public BlockchainType BlockchainType { get; private set; }
    public string RawJson { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public long? Height { get; private set; }
    public string Hash { get; private set; } = string.Empty;
    public int? PeerCount { get; private set; }
    public int? UnconfirmedCount { get; private set; }
    public string Time { get; private set; } = string.Empty;
    public string LatestUrl { get; private set; } = string.Empty;
    public string PreviousHash { get; private set; } = string.Empty;
    public string PreviousUrl { get; private set; } = string.Empty;
    public int? HighFeePerKb { get; private set; }
    public int? MediumFeePerKb { get; private set; }
    public int? LowFeePerKb { get; private set; }
    public int? LastForkHeight { get; private set; }
    public string? LastForkHash { get; private set; }

    private BlockchainSnapshot()
    {
    }

    public static BlockchainSnapshot Create(
        BlockchainType blockchainType,
        string rawJson,
        long? height,
        string hash,
        int? peerCount,
        int? unconfirmedCount,
        DateTimeOffset createdAt,
        string time = "",
        string latestUrl = "",
        string previousHash = "",
        string previousUrl = "",
        int? highFeePerKb = null,
        int? mediumFeePerKb = null,
        int? lowFeePerKb = null,
        int? lastForkHeight = null,
        string? lastForkHash = null)
    {
        return new BlockchainSnapshot
        {
            Id = Guid.NewGuid(),
            BlockchainType = blockchainType,
            RawJson = rawJson,
            CreatedAt = createdAt,
            Height = height,
            Hash = hash,
            PeerCount = peerCount,
            UnconfirmedCount = unconfirmedCount,
            Time = time,
            LatestUrl = latestUrl,
            PreviousHash = previousHash,
            PreviousUrl = previousUrl,
            HighFeePerKb = highFeePerKb,
            MediumFeePerKb = mediumFeePerKb,
            LowFeePerKb = lowFeePerKb,
            LastForkHeight = lastForkHeight,
            LastForkHash = lastForkHash
        };
    }

    public static BlockchainSnapshot LoadFromDb(
        Guid id,
        BlockchainType blockchainType,
        string rawJson,
        DateTimeOffset createdAt,
        long? height,
        string hash,
        int? peerCount,
        int? unconfirmedCount,
        string time = "",
        string latestUrl = "",
        string previousHash = "",
        string previousUrl = "",
        int? highFeePerKb = null,
        int? mediumFeePerKb = null,
        int? lowFeePerKb = null,
        int? lastForkHeight = null,
        string? lastForkHash = null)
    {
        return new BlockchainSnapshot
        {
            Id = id,
            BlockchainType = blockchainType,
            RawJson = rawJson,
            CreatedAt = createdAt,
            Height = height,
            Hash = hash,
            PeerCount = peerCount,
            UnconfirmedCount = unconfirmedCount,
            Time = time,
            LatestUrl = latestUrl,
            PreviousHash = previousHash,
            PreviousUrl = previousUrl,
            HighFeePerKb = highFeePerKb,
            MediumFeePerKb = mediumFeePerKb,
            LowFeePerKb = lowFeePerKb,
            LastForkHeight = lastForkHeight,
            LastForkHash = lastForkHash
        };
    }

    public void Update(
        string rawJson,
        long? height,
        int? peerCount,
        int? unconfirmedCount,
        DateTimeOffset createdAt,
        string? time = null,
        string? latestUrl = null,
        string? previousHash = null,
        string? previousUrl = null,
        int? highFeePerKb = null,
        int? mediumFeePerKb = null,
        int? lowFeePerKb = null,
        int? lastForkHeight = null,
        string? lastForkHash = null)
    {
        RawJson = rawJson;
        Height = height;
        PeerCount = peerCount;
        UnconfirmedCount = unconfirmedCount;

        HighFeePerKb = highFeePerKb;
        MediumFeePerKb = mediumFeePerKb;
        LowFeePerKb = lowFeePerKb;
        LastForkHeight = lastForkHeight;
        LastForkHash = lastForkHash;
        CreatedAt = createdAt;
        if (time is not null)
            Time = time;
        if (latestUrl is not null)
            LatestUrl = latestUrl;
        if (previousHash is not null)
            PreviousHash = previousHash;
        if (previousUrl is not null)
            PreviousUrl = previousUrl;
    }
}