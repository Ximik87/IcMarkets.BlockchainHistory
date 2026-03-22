using System.Net;
using System.Net.Http.Json;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using IcMarkets.BlockchainHistory.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace IcMarkets.BlockchainHistory.FuncTests;

public sealed class BlockchainSnapshotsControllerTests : IClassFixture<BlockchainHistoryWebAppFactory>, IAsyncLifetime
{
    private readonly BlockchainHistoryWebAppFactory _factory;
    private readonly HttpClient _client;

    public BlockchainSnapshotsControllerTests(BlockchainHistoryWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Seed test data
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        db.BlockchainSnapshots.AddRange(
            BlockchainSnapshot.Create(
                BlockchainType.Ethereum,               
                """{"name":"ETH.main","height":100}""",
                height: 100,
                hash: "hash_eth_1",
                peerCount: 50,
                unconfirmedCount: 10),
            BlockchainSnapshot.Create(
                BlockchainType.Ethereum,               
                """{"name":"ETH.main","height":200}""",
                height: 200,
                hash: "hash_eth_2",
                peerCount: 55,
                unconfirmedCount: 12),
            BlockchainSnapshot.Create(
                BlockchainType.BitcoinMain,             
                """{"name":"BTC.main","height":800000}""",
                height: 800000,
                hash: "hash_btc_1",
                peerCount: 300,
                unconfirmedCount: 1000));

        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetTypes_ReturnsAllBlockchainTypes()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/types");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var types = await response.Content.ReadFromJsonAsync<List<string>>();
        types.ShouldNotBeNull();
        types.ShouldContain("Ethereum");
        types.ShouldContain("BitcoinMain");
        types.ShouldContain("Dash");
        types.ShouldContain("Litecoin");
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistory_ValidType_ReturnsSnapshots()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/Ethereum");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<List<BlockchainSnapshotResponse>>();
        snapshots.ShouldNotBeNull();
        snapshots.Count.ShouldBe(2);
        snapshots.ShouldAllBe(s => s.BlockchainType == "Ethereum");
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistory_ValidTypeCaseInsensitive_ReturnsSnapshots()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/ethereum");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<List<BlockchainSnapshotResponse>>();
        snapshots.ShouldNotBeNull();
        snapshots.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetHistory_InvalidType_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/InvalidChain");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetHistory_TypeWithNoData_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/Dash");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<List<BlockchainSnapshotResponse>>();
        snapshots.ShouldNotBeNull();
        snapshots.ShouldBeEmpty();
    }

    [Fact(Skip = "fix later")]
    public async Task GetLatest_ValidType_ReturnsLatestSnapshot()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/Ethereum/latest");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var snapshot = await response.Content.ReadFromJsonAsync<BlockchainSnapshotResponse>();
        snapshot.ShouldNotBeNull();
        snapshot.BlockchainType.ShouldBe("Ethereum");
        snapshot.Height.ShouldBe(200);
    }

    [Fact]
    public async Task GetLatest_InvalidType_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/InvalidChain/latest");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetLatest_TypeWithNoData_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/Litecoin/latest");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistory_BitcoinMain_ReturnsCorrectData()
    {
        // Act
        var response = await _client.GetAsync("/api/BlockchainSnapshots/BitcoinMain");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var snapshots = await response.Content.ReadFromJsonAsync<List<BlockchainSnapshotResponse>>();
        snapshots.ShouldNotBeNull();
        snapshots.Count.ShouldBe(1);
        snapshots[0].Height.ShouldBe(800000);
        snapshots[0].Hash.ShouldBe("hash_btc_1");
    }
}