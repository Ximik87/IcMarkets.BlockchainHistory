using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands;

public sealed record UpdateBlockchainSnapshotCommand(BlockchainSnapshot Blockchain) : ICommand;

public sealed class UpdateBlockchainSnapshotCommandHandler : ICommandHandler<UpdateBlockchainSnapshotCommand>
{
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBlockchainSnapshotCommandHandler(
        IBlockchainSnapshotRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(UpdateBlockchainSnapshotCommand command,
        CancellationToken cancellationToken)
    {
        _repository.Update(command.Blockchain);
        //await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}