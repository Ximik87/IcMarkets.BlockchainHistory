using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;

public sealed record SaveBlockchainSnapshotCommand(BlockchainSnapshot Blockchain) : ICommand;

public sealed class SaveBlockchainSnapshotCommandHandler : ICommandHandler<SaveBlockchainSnapshotCommand>
{
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveBlockchainSnapshotCommandHandler(
        IBlockchainSnapshotRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(SaveBlockchainSnapshotCommand command, CancellationToken cancellationToken)
    {
        _repository.Add(command.Blockchain);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}