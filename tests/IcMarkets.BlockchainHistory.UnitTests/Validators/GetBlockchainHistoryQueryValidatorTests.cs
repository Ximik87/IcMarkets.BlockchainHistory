using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class GetBlockchainHistoryQueryValidatorTests
{
    private readonly GetBlockchainHistoryQueryValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Query_Is_Valid()
    {
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_BlockchainType_Is_Invalid()
    {
        var query = new GetBlockchainHistoryQuery((BlockchainType)999, DateTime.UtcNow);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.BlockchainType);
    }

    [Fact]
    public void Should_Fail_When_Time_Is_Default()
    {
        var query = new GetBlockchainHistoryQuery(BlockchainType.Ethereum, default);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Time);
    }
}
