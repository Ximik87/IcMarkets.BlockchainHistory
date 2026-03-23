using FluentValidation;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;

public sealed class SaveBlockchainSnapshotCommandValidator : AbstractValidator<SaveBlockchainSnapshotCommand>
{
    public SaveBlockchainSnapshotCommandValidator()
    {
        RuleFor(x => x.Blockchain)
            .NotNull();

        RuleFor(x => x.Blockchain.Hash)
            .NotEmpty()
            .When(x => x.Blockchain is not null);

        RuleFor(x => x.Blockchain.RawJson)
            .NotEmpty()
            .When(x => x.Blockchain is not null);
    }
}

public sealed class UpdateBlockchainSnapshotCommandValidator : AbstractValidator<UpdateBlockchainSnapshotCommand>
{
    public UpdateBlockchainSnapshotCommandValidator()
    {
        RuleFor(x => x.Blockchain)
            .NotNull();

        RuleFor(x => x.Blockchain.Hash)
            .NotEmpty()
            .When(x => x.Blockchain is not null);

        RuleFor(x => x.Blockchain.RawJson)
            .NotEmpty()
            .When(x => x.Blockchain is not null);
    }
}