namespace IcMarkets.BlockchainHistory.Application.DTOs;

public sealed class Blockchain
{
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string LatestUrl { get; set; } = string.Empty;
    public string PreviousHash { get; set; } = string.Empty;
    public string PreviousUrl { get; set; } = string.Empty;
    public int PeerCount { get; set; }
    public int UnconfirmedCount { get; set; }
    public int HighFeePerKb { get; set; }
    public int MediumFeePerKb { get; set; }
    public int LowFeePerKb { get; set; }
    public int LastForkHeight { get; set; }
    public string? LastForkHash { get; set; }
}