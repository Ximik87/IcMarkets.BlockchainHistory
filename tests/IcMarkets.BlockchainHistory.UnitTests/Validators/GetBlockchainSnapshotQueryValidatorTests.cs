using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class GetBlockchainSnapshotQueryValidatorTests
{
    private readonly GetBlockchainSnapshotQueryValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Hash_Is_Provided_Test()
    {
        // Arrange
        var query = new GetBlockchainSnapshotQuery("abc123");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Fail_When_Hash_Is_Empty_Or_Null_Test(string? hash)
    {
        // Arrange
        var query = new GetBlockchainSnapshotQuery(hash!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Hash);
    }
}
