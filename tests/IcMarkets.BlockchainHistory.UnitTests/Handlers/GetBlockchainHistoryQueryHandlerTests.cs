using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;

namespace IcMarkets.BlockchainHistory.UnitTests.Handlers;

public sealed class GetBlockchainHistoryQueryHandlerTests : IDisposable
{
    private readonly Mock<IBlockchainSnapshotRepository> _repositoryMock = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly GetBlockchainHistoryQueryHandler _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetBlockchainHistoryQueryHandlerTests()
    {
        _handler = new GetBlockchainHistoryQueryHandler(_repositoryMock.Object, _cache);
    }

    [Fact]
    public async Task Handle_ShouldReturnSnapshotsFromRepository_WhenCacheIsEmpty_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "abc123", 10, 5, now);
        _repositoryMock
            .Setup(r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<BlockchainSnapshot> { snapshot }, 1));
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, now.UtcDateTime);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.TotalCount.ShouldBe(1);
        result.Items[0].BlockchainType.ShouldBe(nameof(BlockchainType.Ethereum));
        result.Items[0].Height.ShouldBe(100);
        result.Items[0].Hash.ShouldBe("abc123");
        result.Items[0].PeerCount.ShouldBe(10);
        result.Items[0].UnconfirmedCount.ShouldBe(5);
        _repositoryMock.Verify(
            r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoSnapshots_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        _repositoryMock
            .Setup(r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<BlockchainSnapshot>(), 0));
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, now.UtcDateTime);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldQueryRepository_ForDifferentBlockchainTypes_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var ethSnapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "eth123", 10, 5, now);
        var btcSnapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain, "{}", 200, "btc456", 20, 10, now);
        _repositoryMock
            .Setup(r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<BlockchainSnapshot> { ethSnapshot }, 1));
        _repositoryMock
            .Setup(r => r.GetHistoryAsync(BlockchainType.BitcoinMain, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<BlockchainSnapshot> { btcSnapshot }, 1));
        var ethQuery = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, now.UtcDateTime);
        var btcQuery = new GetBlockchainHistoryQuery(BlockchainType.BitcoinMain, now.UtcDateTime);

        // Act
        var ethResult = await _handler.Handle(ethQuery, _token);
        var btcResult = await _handler.Handle(btcQuery, _token);

        // Assert
        ethResult.Items.Count.ShouldBe(1);
        ethResult.Items[0].Hash.ShouldBe("eth123");
        btcResult.Items.Count.ShouldBe(1);
        btcResult.Items[0].Hash.ShouldBe("btc456");
        _repositoryMock.Verify(
            r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _repositoryMock.Verify(
            r => r.GetHistoryAsync(BlockchainType.BitcoinMain, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAllFieldsCorrectly_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{\"key\":\"value\"}", 999, "hash999", 42, 7, now);
        _repositoryMock
            .Setup(r => r.GetHistoryAsync(BlockchainType.Ethereum, It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<BlockchainSnapshot> { snapshot }, 1));
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, now.UtcDateTime);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        var response = result.Items.ShouldHaveSingleItem();
        response.Id.ShouldBe(snapshot.Id);
        response.BlockchainType.ShouldBe("Ethereum");
        response.RawJson.ShouldBe("{\"key\":\"value\"}");
        response.CreatedAt.ShouldBe(now);
        response.Height.ShouldBe(999);
        response.Hash.ShouldBe("hash999");
        response.PeerCount.ShouldBe(42);
        response.UnconfirmedCount.ShouldBe(7);
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}