using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;

namespace IcMarkets.BlockchainHistory.Infrastructure.External;

internal sealed class BlockCypherClient : IBlockCypherClient
{
    private readonly IHttpClientFactory _factory;

    public BlockCypherClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task Get()
    {
        using var client = _factory.CreateClient("BlockCypher");
        var response =
            await client.GetFromJsonAsync<Blockchain>("https://api.blockcypher.com/v1/btc/main");

        Console.WriteLine(response?.name);
    }
}

public class Blockchain
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