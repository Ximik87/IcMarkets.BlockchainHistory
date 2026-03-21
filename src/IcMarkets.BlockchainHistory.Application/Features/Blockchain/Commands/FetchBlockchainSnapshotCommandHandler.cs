using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Domain.Entities;
using Mediator;

namespace IcMarkets.BlockchainHistory.Application.Features.Blockchain.Commands
{
    public sealed class FetchBlockchainSnapshotCommandHandler : ICommandHandler<FetchBlockchainSnapshotCommand>
    {
        private readonly IBlockCypherClient _client;
        private readonly IBlockchainSnapshotRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public FetchBlockchainSnapshotCommandHandler(
            IBlockCypherClient client,
            IBlockchainSnapshotRepository repository,
            IUnitOfWork unitOfWork)
        {
            _client = client;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Unit> Handle(FetchBlockchainSnapshotCommand command, CancellationToken cancellationToken)
        {
            var data = await _client.GetAsync(command.BlockchainType, cancellationToken);
            var rawJson = JsonSerializer.Serialize(data);

            var snapshot = BlockchainSnapshot.Create(
                command.BlockchainType,
                string.Empty,
                rawJson,
                data.Height,
                data.Hash,
                data.PeerCount,
                data.UnconfirmedCount);

            await _repository.AddAsync(snapshot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
