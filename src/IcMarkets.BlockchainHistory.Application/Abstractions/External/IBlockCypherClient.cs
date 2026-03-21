using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Application.Abstractions.External;

public interface IBlockCypherClient
{
    Task<Blockchain> GetAsync(BlockchainType blockchainType, CancellationToken ct);    
}