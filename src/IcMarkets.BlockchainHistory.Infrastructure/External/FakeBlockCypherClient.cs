using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Infrastructure.External;

internal sealed class FakeBlockCypherClient : IBlockCypherClient
{
    public Task<Blockchain> GetAsync(BlockchainType blockchainType, CancellationToken ct)
    {
        var body = GetUrl(blockchainType);
        var response = JsonSerializer.Deserialize<BlockchainResponse>(body);

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

        return Task.FromResult(blockchain);
    }

    private string GetUrl(BlockchainType blockchainType) => blockchainType switch
    {
        BlockchainType.Ethereum => GetEthBody(),
        BlockchainType.Dash => GetDashBody(),
        BlockchainType.BitcoinMain => GetBtcMainBody(),
        BlockchainType.BitcoinTest3 => GetBtcTest3Body(),
        BlockchainType.Litecoin => GetLtcBody(),
        _ => throw new ArgumentOutOfRangeException(nameof(blockchainType), blockchainType, null)
    };

    private string GetDashBody()
    {
        return
            "{\"hash\": \"000000000000000476c56129426fea53063c8fcb19d69361376d47913a9082d8\", \"name\": \"DASH.main\", \"time\": \"2026-03-23T07:23:38.032525324Z\", \"height\": 2442657, \"latest_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/000000000000000476c56129426fea53063c8fcb19d69361376d47913a9082d8\", \"peer_count\": 122, \"low_fee_per_kb\": 5492, \"previous_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/0000000000000008cfc6930a65e212c26fa09279bad843b34b9f13056d23d865\", \"high_fee_per_kb\": 22748, \"last_fork_hash\": \"000000000000000c60814219ff6839ad38c103141fab1309869c2ce02165f5f6\", \"previous_hash\": \"0000000000000008cfc6930a65e212c26fa09279bad843b34b9f13056d23d865\", \"last_fork_height\": 2438255, \"medium_fee_per_kb\": 15391, \"unconfirmed_count\": 220}";
    }

    private string GetEthBody()
    {
        return
            "{\"hash\": \"6c995bc394a616e900ad8fed90e08e110f471f13ff75b26e757f0f3002eb7926\", \"name\": \"ETH.main\", \"time\": \"2026-03-23T07:26:13.931457325Z\", \"height\": 24718714, \"latest_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/6c995bc394a616e900ad8fed90e08e110f471f13ff75b26e757f0f3002eb7926\", \"peer_count\": 0, \"low_fee_per_kb\": 0, \"previous_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/0002c78e7724022ed6be20d1c62a17e789ccd418f61caa2b46adf275fc5f2e50\", \"high_fee_per_kb\": 0, \"last_fork_hash\": \"2ad00c414d8ae13be7c7ccfd08a2067d8f84c7af0a129a93b3bc74a4a9c19a9b\", \"previous_hash\": \"0002c78e7724022ed6be20d1c62a17e789ccd418f61caa2b46adf275fc5f2e50\", \"last_fork_height\": 24714759, \"medium_fee_per_kb\": 0, \"unconfirmed_count\": 4}";
    }

    private string GetBtcMainBody()
    {
        return
            "{\"hash\": \"000000000000000000012a561edd0d912d084802ef3584630353516b7bc73a55\", \"name\": \"BTC.main\", \"time\": \"2026-03-23T06:57:45.724782361Z\", \"height\": 941829, \"latest_url\": \"https://api.blockcypher.com/v1/btc/main/blocks/000000000000000000012a561edd0d912d084802ef3584630353516b7bc73a55\", \"peer_count\": 324, \"low_fee_per_kb\": 1426, \"previous_url\": \"https://api.blockcypher.com/v1/btc/main/blocks/00000000000000000000180264607e895334c66c4ca8c747a782a9781777fc64\", \"high_fee_per_kb\": 2611, \"last_fork_hash\": \"00000000000000000000626a49c9c9047fb8b49a8216daf5e448d44517d38a55\", \"previous_hash\": \"00000000000000000000180264607e895334c66c4ca8c747a782a9781777fc64\", \"last_fork_height\": 935976, \"medium_fee_per_kb\": 1677, \"unconfirmed_count\": 3245}";
    }

    private string GetBtcTest3Body()
    {
        return
            "{\"hash\": \"00000000000aa760fffa5f1e1336f1ee9450eed5c25a6b7fe3b3d9d01655e364\", \"name\": \"BTC.test3\", \"time\": \"2026-03-22T21:01:41.488764341Z\", \"height\": 4786130, \"latest_url\": \"https://api.blockcypher.com/v1/btc/test3/blocks/00000000000aa760fffa5f1e1336f1ee9450eed5c25a6b7fe3b3d9d01655e364\", \"peer_count\": 202, \"low_fee_per_kb\": 7553, \"previous_url\": \"https://api.blockcypher.com/v1/btc/test3/blocks/000000000474c9ec4976e9aaad6b4b58811799dd77ec08879abb756e2c8b7e87\", \"high_fee_per_kb\": 23991, \"last_fork_hash\": \"0000000008988f607ec81c80952d559f34fecfc1cb938969509ed144bcf8a86e\", \"previous_hash\": \"000000000474c9ec4976e9aaad6b4b58811799dd77ec08879abb756e2c8b7e87\", \"last_fork_height\": 4786122, \"medium_fee_per_kb\": 13142, \"unconfirmed_count\": 0}";
    }

    private string GetLtcBody()
    {
        return
            "{\"hash\": \"d0df3ea997724a5e24a1bd0e357ef091bdf5879c2608e749be7aad23c7b9ddcd\", \"name\": \"LTC.main\", \"time\": \"2026-03-22T21:02:07.785060501Z\", \"height\": 3076247, \"latest_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/d0df3ea997724a5e24a1bd0e357ef091bdf5879c2608e749be7aad23c7b9ddcd\", \"peer_count\": 322, \"low_fee_per_kb\": 7141, \"previous_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/95049d18a60acc3ed64645d1014d4d8f447e60be004300738ea0f35b9b83e80c\", \"high_fee_per_kb\": 12303, \"last_fork_hash\": \"a369d55bc5effc68c25d02c2c4ab05c3f9ba69694d97b4f1c9a9771c4faaca86\", \"previous_hash\": \"95049d18a60acc3ed64645d1014d4d8f447e60be004300738ea0f35b9b83e80c\", \"last_fork_height\": 3076191, \"medium_fee_per_kb\": 9159, \"unconfirmed_count\": 21}";
    }
}