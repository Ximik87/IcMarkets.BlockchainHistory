namespace IcMarkets.BlockchainHistory.Infrastructure;


public class BlockCypherOptions
{
    public string EthMainUrl { get; set; } = default!;
    public string DashMainUrl { get; set; } = default!;
    public string BtcMainUrl { get; set; } = default!;
    public string BtcTest3Url { get; set; } = default!;
    public string LtcMainUrl { get; set; } = default!;
}