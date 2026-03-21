using IcMarkets.BlockchainHistory.Domain.Enums;
using IcMarkets.BlockchainHistory.Infrastructure.External;
using Microsoft.Extensions.Options;
using Moq;

namespace IcMarkets.BlockchainHistory.IntegrationTests;

public sealed class BlockCypherClientTests
{
    [Fact]
    public async Task Get_Test()
    {
        // Arrange
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(x => x.CreateClient(""))
            .Returns(() => new HttpClient());
        var options = Options.Create(new BlockCypherOptions
        {
            BtcMainUrl = "https://api.blockcypher.com/v1/btc/main",
            BtcTest3Url = "https://api.blockcypher.com/v1/btc/test3",
            LtcMainUrl = "https://api.blockcypher.com/v1/ltc/main",
            DashMainUrl = "https://api.blockcypher.com/v1/dash/main",
            EthMainUrl = "https://api.blockcypher.com/v1/eth/main"
        });
        var sut = new BlockCypherClient(clientFactory.Object, options);

        // Act
        var result = await sut.GetAsync(BlockchainType.BitcoinMain, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}