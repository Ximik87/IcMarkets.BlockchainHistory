using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Infrastructure.External;
using Moq;

namespace IcMarkets.BlockchainHistory.IntegrationTests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(x => x.CreateClient("BlockCypher")).Returns(() => new HttpClient());

        var sut = new BlockCypherClient(clientFactory.Object);

        await sut.Get();
    }
}