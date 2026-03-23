using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Moq;

namespace IcMarkets.BlockchainHistory.UnitTests.Handlers;

public sealed class SaveBlockchainSnapshotCommandHandlerTests
{
    private readonly Mock<IBlockchainSnapshotRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly SaveBlockchainSnapshotCommandHandler _handler;

    public SaveBlockchainSnapshotCommandHandlerTests()
    {
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _handler = new SaveBlockchainSnapshotCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddSnapshotToRepository_Test()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "abc123", 10, 5,
            DateTimeOffset.UtcNow);
        var command = new SaveBlockchainSnapshotCommand(snapshot);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Add(snapshot), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}