using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class GetLatestBlockchainSnapshotQueryValidatorTests
{
    private readonly GetLatestBlockchainSnapshotQueryValidator _validator = new();

    [Fact]
    public void Should_Pass_When_BlockchainType_Is_Valid_Test()
    {
        // Arrange
        var query = new GetLatestBlockchainSnapshotQuery(BlockchainType.Ethereum);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_BlockchainType_Is_Invalid_Test()
    {
        // Arrange
        var query = new GetLatestBlockchainSnapshotQuery((BlockchainType)999);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BlockchainType);
    }
}
