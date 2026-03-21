using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Microsoft.Extensions.Options;

namespace IcMarkets.BlockchainHistory.Infrastructure.External;

internal sealed class BlockCypherClient : IBlockCypherClient
{
    private readonly IHttpClientFactory _factory;
    private readonly BlockCypherOptions _options;

    public BlockCypherClient(IHttpClientFactory factory, IOptions<BlockCypherOptions> options)
    {
        _factory = factory;
        _options = options.Value;
    }

    public async Task<Blockchain> GetAsync(BlockchainType blockchainType, CancellationToken ct)
    {
        var url = GetUrl(blockchainType);
        using var client = _factory.CreateClient();
        var response = await client.GetFromJsonAsync<BlockchainResponse>(url, ct);

        var blockchain = new Blockchain
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

        return blockchain;
    }
  
    private string GetUrl(BlockchainType blockchainType) => blockchainType switch
    {
        BlockchainType.Ethereum => _options.EthMainUrl,
        BlockchainType.Dash => _options.DashMainUrl,
        BlockchainType.BitcoinMain => _options.BtcMainUrl,
        BlockchainType.BitcoinTest3 => _options.BtcTest3Url,
        BlockchainType.Litecoin => _options.LtcMainUrl,
        _ => throw new ArgumentOutOfRangeException(nameof(blockchainType), blockchainType, null)
    };
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class BlockchainResponse
{
    public string name { get; set; } = string.Empty;
    public int height { get; set; }
    public string hash { get; set; } = string.Empty;
    public string time { get; set; } = string.Empty;
    public string latest_url { get; set; } = string.Empty;
    public string previous_hash { get; set; } = string.Empty;
    public string previous_url { get; set; } = string.Empty;
    public int peer_count { get; set; }
    public int unconfirmed_count { get; set; }
    public int high_fee_per_kb { get; set; }
    public int medium_fee_per_kb { get; set; }
    public int low_fee_per_kb { get; set; }
    public int last_fork_height { get; set; }
    public string? last_fork_hash { get; set; }
}