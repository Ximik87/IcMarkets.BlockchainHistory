using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class GetBlockchainHistoryQueryValidatorTests
{
    private readonly GetBlockchainHistoryQueryValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Query_Is_Valid_Test()
    {
        // Arrange
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, DateTime.UtcNow);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_BlockchainType_Is_Invalid_Test()
    {
        // Arrange
        var query = new GetBlockchainHistoryQuery((BlockchainType)999, DateTime.UtcNow);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BlockchainType);
    }

    [Fact]
    public void Should_Fail_When_Time_Is_Default_Test()
    {
        // Arrange
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, default);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Time);
    }
}
