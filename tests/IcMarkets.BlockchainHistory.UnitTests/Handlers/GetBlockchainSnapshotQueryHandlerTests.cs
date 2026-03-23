using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Moq;
using Shouldly;

namespace IcMarkets.BlockchainHistory.UnitTests.Handlers;

public sealed class GetBlockchainSnapshotQueryHandlerTests
{
    private readonly Mock<IBlockchainSnapshotRepository> _repositoryMock = new();
    private readonly GetBlockchainSnapshotQueryHandler _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetBlockchainSnapshotQueryHandlerTests()
    {
        _handler = new GetBlockchainSnapshotQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSnapshot_WhenRepositoryFindsMatchingHash_Test()
    {
        // Arrange
        const string hash = "abc123";
        var now = DateTimeOffset.UtcNow;
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, hash, 10, 5, now);
        _repositoryMock
            .Setup(r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);
        var query = new GetBlockchainSnapshotQuery(hash);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldNotBeNull();
        result.Hash.ShouldBe(hash);
        result.Height.ShouldBe(100);
        result.PeerCount.ShouldBe(10);
        result.UnconfirmedCount.ShouldBe(5);
        _repositoryMock.Verify(
            r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull_Test()
    {
        // Arrange
        const string hash = "nonexistent";
        _repositoryMock
            .Setup(r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        var query = new GetBlockchainSnapshotQuery(hash);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.ShouldBeNull();
        _repositoryMock.Verify(
            r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectHashToRepository_Test()
    {
        // Arrange
        const string hash = "specific-hash-value";
        _repositoryMock
            .Setup(r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        var query = new GetBlockchainSnapshotQuery(hash);

        // Act
        await _handler.Handle(query, _token);

        // Assert
        _repositoryMock.Verify(
            r => r.GetByHashAsync(hash, It.IsAny<CancellationToken>()),
            Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}