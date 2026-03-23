using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Shouldly;

namespace IcMarkets.BlockchainHistory.UnitTests.Features;

public sealed class BlockchainSnapshotSynchronizerTests
{
    private readonly Mock<IBlockCypherClient> _clientMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly BlockchainSnapshotSynchronizer _sut;
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);

    public BlockchainSnapshotSynchronizerTests()
    {
        var timeProvider = new FakeTimeProvider(FixedUtcNow);
        _sut = new BlockchainSnapshotSynchronizer(
            _clientMock.Object,
            _mediatorMock.Object,
            timeProvider);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldSaveNewSnapshot_WhenSnapshotDoesNotExist_Test()
    {
        // Arrange
        var blockchainData = CreateBlockchainData();
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blockchainData);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBlockchainSnapshotQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SaveBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        await _sut.FetchAndStoreAsync(BlockchainType.BitcoinMain, CancellationToken.None);

        // Assert
        _clientMock.Verify(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetBlockchainSnapshotQuery>(q => q.Hash == blockchainData.Hash),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<SaveBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<UpdateBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldUpdateExistingSnapshot_WhenSnapshotAlreadyExists_Test()
    {
        // Arrange
        var blockchainData = CreateBlockchainData();
        var existingSnapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            "{}",
            blockchainData.Height,
            blockchainData.Hash,
            blockchainData.PeerCount,
            blockchainData.UnconfirmedCount,
            FixedUtcNow.AddHours(-1));
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blockchainData);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBlockchainSnapshotQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSnapshot);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        await _sut.FetchAndStoreAsync(BlockchainType.BitcoinMain, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<UpdateBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<SaveBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldUpdateSnapshotFields_WhenSnapshotAlreadyExists_Test()
    {
        // Arrange
        var blockchainData = CreateBlockchainData(height: 900_000, peerCount: 500, unconfirmedCount: 200);
        var existingSnapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            "{}",
            800_000,
            blockchainData.Hash,
            100,
            50,
            FixedUtcNow.AddHours(-1));
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blockchainData);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBlockchainSnapshotQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSnapshot);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        await _sut.FetchAndStoreAsync(BlockchainType.BitcoinMain, CancellationToken.None);

        // Assert
        existingSnapshot.Height.ShouldBe(900_000);
        existingSnapshot.PeerCount.ShouldBe(500);
        existingSnapshot.UnconfirmedCount.ShouldBe(200);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldCallClientWithCorrectBlockchainType_Test()
    {
        // Arrange
        var blockchainData = CreateBlockchainData();
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blockchainData);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBlockchainSnapshotQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SaveBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        await _sut.FetchAndStoreAsync(BlockchainType.Ethereum, CancellationToken.None);

        // Assert
        _clientMock.Verify(c => c.GetAsync(BlockchainType.Ethereum, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldQueryByHash_WhenLookingUpExistingSnapshot_Test()
    {
        // Arrange
        const string expectedHash = "abc123hash";
        var blockchainData = CreateBlockchainData(hash: expectedHash);
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blockchainData);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBlockchainSnapshotQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockchainSnapshot?)null);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SaveBlockchainSnapshotCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        await _sut.FetchAndStoreAsync(BlockchainType.BitcoinMain, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetBlockchainSnapshotQuery>(q => q.Hash == expectedHash), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FetchAndStoreAsync_ShouldPropagateException_WhenClientThrows_Test()
    {
        // Arrange
        _clientMock
            .Setup(c => c.GetAsync(BlockchainType.BitcoinMain, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() =>
            _sut.FetchAndStoreAsync(BlockchainType.BitcoinMain, CancellationToken.None));
    }

    private static Blockchain CreateBlockchainData(
        string hash = "0000000000000000000abc123",
        int height = 850_000,
        int peerCount = 250,
        int unconfirmedCount = 100)
    {
        return new Blockchain
        {
            Name = "BTC.main",
            Height = height,
            Hash = hash,
            Time = "2024-06-15T12:00:00Z",
            LatestUrl = "https://api.blockcypher.com/v1/btc/main/blocks/abc123",
            PreviousHash = "0000000000000000000prev",
            PreviousUrl = "https://api.blockcypher.com/v1/btc/main/blocks/prev",
            PeerCount = peerCount,
            UnconfirmedCount = unconfirmedCount,
            HighFeePerKb = 50000,
            MediumFeePerKb = 25000,
            LowFeePerKb = 10000,
            LastForkHeight = 849_999,
            LastForkHash = "0000000000000000000fork"
        };
    }
}