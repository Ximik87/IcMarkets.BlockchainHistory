using IcMarkets.BlockchainHistory.Application.DTOs;

namespace IcMarkets.BlockchainHistory.Application.Abstractions.External;

public interface IBlockCypherClient
{
    Task<Blockchain> Get();
}