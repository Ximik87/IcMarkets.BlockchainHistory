namespace IcMarkets.BlockchainHistory.Application.DTOs;

public sealed class Blockchain
{
    public string Name { get; set; }
    public int Height { get; set; }
    public string Hash { get; set; }
    public string Time { get; set; }
    public string LatestUrl { get; set; }
    public string PreviousHash { get; set; }
    public string PreviousUrl { get; set; }
    public int PeerCount { get; set; }
    public int UnconfirmedCount { get; set; }
    public int HighFeePerKb { get; set; }
    public int MediumFeePerKb { get; set; }
    public int LowFeePerKb { get; set; }
    public int LastForkHeight { get; set; }
    public string LastForkHash { get; set; }
}