using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;

namespace IcMarkets.BlockchainHistory.UnitTests.Handlers;

public sealed class GetLatestBlockchainSnapshotQueryHandlerTests : IDisposable
{
    private readonly Mock<IBlockchainSnapshotRepository> _repositoryMock = new();
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly GetLatestBlockchainSnapshotQueryHandler _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetLatestBlockchainSnapshotQueryHandlerTests()
    {
        _handler = new GetLatestBlockchainSnapshotQueryHandler(_repositoryMock.Object, _cache);
    }

    [Fact]
    public async Task Handle_ShouldReturnSnapshotFromRepository_WhenCacheIsEmpty_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "abc123", 10, 5, now);
        _repositoryMock
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldNotBeNull();
        result.BlockchainType.ShouldBe(nameof(BlockchainType.Ethereum));
        result.Height.ShouldBe(100);
        result.Hash.ShouldBe("abc123");
        result.PeerCount.ShouldBe(10);
        result.UnconfirmedCount.ShouldBe(5);
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull_Test()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldBeNull();
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResponse_WhenCalledTwice_Test()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "abc123", 10, 5, now);
        _repositoryMock
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result1 = await _handler.Handle(query, _token);
        var result2 = await _handler.Handle(query, _token);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Hash.ShouldBe(result2.Hash);
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCacheNullResponse_WhenRepositoryReturnsNull_Test()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result1 = await _handler.Handle(query, _token);
        var result2 = await _handler.Handle(query, _token);

        // Assert
        result1.ShouldBeNull();
        result2.ShouldBeNull();
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()),
            Times.Once);
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
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ethSnapshot);
        _repositoryMock
            .Setup(r => r.GetLatestAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(btcSnapshot);
        var ethQuery = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);
        var btcQuery = new GetLatestBlockchainSnapshotQuery(BlockchainType.BitcoinMain);

        // Act
        var ethResult = await _handler.Handle(ethQuery, _token);
        var btcResult = await _handler.Handle(btcQuery, _token);

        // Assert
        ethResult.ShouldNotBeNull();
        ethResult.Hash.ShouldBe("eth123");
        btcResult.ShouldNotBeNull();
        btcResult.Hash.ShouldBe("btc456");
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()),
            Times.Once);
        _repositoryMock.Verify(
            r => r.GetLatestAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()),
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
            .Setup(r => r.GetLatestAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(snapshot.Id);
        result.BlockchainType.ShouldBe("Ethereum");
        result.RawJson.ShouldBe("{\"key\":\"value\"}");
        result.CreatedAt.ShouldBe(now);
        result.Height.ShouldBe(999);
        result.Hash.ShouldBe("hash999");
        result.PeerCount.ShouldBe(42);
        result.UnconfirmedCount.ShouldBe(7);
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
