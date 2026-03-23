using FluentValidation.TestHelper;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;

namespace IcMarkets.BlockchainHistory.UnitTests.Validators;

public sealed class GetBlockchainSnapshotQueryValidatorTests
{
    private readonly GetBlockchainSnapshotQueryValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Hash_Is_Provided()
    {
        var query = new GetBlockchainSnapshotQuery("abc123");

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Fail_When_Hash_Is_Empty_Or_Null(string? hash)
    {
        var query = new GetBlockchainSnapshotQuery(hash!);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Hash);
    }
}
