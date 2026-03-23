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
    }
}
