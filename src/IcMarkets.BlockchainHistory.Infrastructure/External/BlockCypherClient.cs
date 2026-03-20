using System.Net.Http.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.DTOs;

namespace IcMarkets.BlockchainHistory.Infrastructure.External;

internal sealed class BlockCypherClient : IBlockCypherClient
{
    private readonly IHttpClientFactory _factory;

    public BlockCypherClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<Blockchain> Get()
    {
        using var client = _factory.CreateClient();
        var response =
            await client.GetFromJsonAsync<BlockchainResponse>("https://api.blockcypher.com/v1/btc/main");

        return new Blockchain
        {
            Name = response?.name ?? string.Empty,
            Height = response?.height ?? 0,
            Hash = response?.hash ?? string.Empty,
            Time = response?.time ?? string.Empty,
            LatestUrl = response?.latest_url ?? string.Empty,
            PreviousHash = response?.previous_hash ?? string.Empty,
            PreviousUrl = response?.previous_url ?? string.Empty,
            PeerCount = response?.peer_count ?? 0,
            UnconfirmedCount = response?.unconfirmed_count ?? 0,
            HighFeePerKb = response?.high_fee_per_kb ?? 0,
            MediumFeePerKb = response?.medium_fee_per_kb ?? 0,
            LowFeePerKb = response?.low_fee_per_kb ?? 0,
            LastForkHeight = response?.last_fork_height ?? 0,
            LastForkHash = response?.last_fork_hash ?? string.Empty
        };
    }
}

internal class BlockchainResponse
{
    public string name { get; set; }
    public int height { get; set; }
    public string hash { get; set; }
    public string time { get; set; }
    public string latest_url { get; set; }
    public string previous_hash { get; set; }
    public string previous_url { get; set; }
    public int peer_count { get; set; }
    public int unconfirmed_count { get; set; }
    public int high_fee_per_kb { get; set; }
    public int medium_fee_per_kb { get; set; }
    public int low_fee_per_kb { get; set; }
    public int last_fork_height { get; set; }
    public string last_fork_hash { get; set; }
}