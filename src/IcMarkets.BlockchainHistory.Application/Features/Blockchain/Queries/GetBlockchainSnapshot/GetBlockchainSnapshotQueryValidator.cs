using FluentValidation;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainSnapshot;

public sealed class GetBlockchainSnapshotQueryValidator : AbstractValidator<GetBlockchainSnapshotQuery>
{
    public GetBlockchainSnapshotQueryValidator()
    {
        RuleFor(x => x.Hash)
            .NotEmpty();
    }
}
