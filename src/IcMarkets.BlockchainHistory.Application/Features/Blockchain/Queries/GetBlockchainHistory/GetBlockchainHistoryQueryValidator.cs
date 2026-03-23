using FluentValidation;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;

public sealed class GetBlockchainHistoryQueryValidator : AbstractValidator<GetBlockchainHistoryQuery>
{
    public GetBlockchainHistoryQueryValidator()
    {
        RuleFor(x => x.BlockchainType)
            .IsInEnum();

        RuleFor(x => x.Time)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
