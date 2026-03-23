using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;
using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class SaveBlockchainSnapshotCommandValidatorTests
{
    private readonly SaveBlockchainSnapshotCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Command_Is_Valid_Test()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, "abc123", 10, 5);

        var command = new SaveBlockchainSnapshotCommand(snapshot);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Blockchain_Is_Null_Test()
    {
        // Arrange
        var command = new SaveBlockchainSnapshotCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blockchain);
    }

    [Fact]
    public void Should_Fail_When_Hash_Is_Empty_Test()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, "{}", 100, string.Empty, 10, 5);

        var command = new SaveBlockchainSnapshotCommand(snapshot);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blockchain.Hash);
    }

    [Fact]
    public void Should_Fail_When_RawJson_Is_Empty_Test()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum, string.Empty, 100, "abc123", 10, 5);

        var command = new SaveBlockchainSnapshotCommand(snapshot);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Blockchain.RawJson);
    }
}
