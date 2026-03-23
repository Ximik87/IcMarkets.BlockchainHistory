using FluentValidation;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetLatestBlockchainSnapshot;

public sealed class GetLatestBlockchainSnapshotQueryValidator : AbstractValidator<GetLatestBlockchainSnapshotQuery>
{
    public GetLatestBlockchainSnapshotQueryValidator()
    {
        RuleFor(x => x.BlockchainType)
            .IsInEnum();
    }
}
