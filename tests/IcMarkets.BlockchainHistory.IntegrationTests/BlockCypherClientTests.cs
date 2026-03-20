using IcMarkets.BlockchainHistory.Infrastructure.External;
using Moq;

namespace IcMarkets.BlockchainHistory.IntegrationTests;

public class BlockCypherClientTests
{
    [Fact]
    public async Task Get_Test()
    {
        // Arrange
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(x => x.CreateClient(""))
            .Returns(() => new HttpClient());
        var sut = new BlockCypherClient(clientFactory.Object);

        // Act
        var result = await sut.Get();

        // Assert
        Assert.NotNull(result);
    }
}