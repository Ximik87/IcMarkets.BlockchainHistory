using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.DTOs;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain.Queries.GetBlockchainHistory;
using IcMarkets.BlockchainHistory.Domain.Enums;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace IcMarkets.BlockchainHistory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BlockchainSnapshotsController : ControllerBase
{
    private readonly IBlockchainSnapshotRepository _repository;
    private readonly IMediator _mediator;

    public BlockchainSnapshotsController(
        IBlockchainSnapshotRepository repository,
        IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the history of blockchain snapshots for a specific blockchain type, ordered by CreatedAt descending.
    /// </summary>
    [HttpGet("{blockchainType}")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHistory(
        string blockchainType,
        CancellationToken ct)
    {
        if (!Enum.TryParse<BlockchainType>(blockchainType, ignoreCase: true, out var parsed))
        {
            return BadRequest($"Invalid blockchain type '{blockchainType}'. " +
                              $"Valid values: {string.Join(", ", Enum.GetNames<BlockchainType>())}");
        }

        var response = await _mediator.Send(new GetBlockchainHistoryQuery(parsed), ct);

        return Ok(response);
    }

    /// <summary>
    /// Gets the latest snapshot for a specific blockchain type.
    /// </summary>
    [HttpGet("{blockchainType}/latest")]
    [ProducesResponseType(typeof(BlockchainSnapshotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatest(
        string blockchainType,
        CancellationToken ct)
    {
        if (!Enum.TryParse<BlockchainType>(blockchainType, ignoreCase: true, out var parsed))
        {
            return BadRequest($"Invalid blockchain type '{blockchainType}'. " +
                              $"Valid values: {string.Join(", ", Enum.GetNames<BlockchainType>())}");
        }

        var snapshot = await _repository.GetLatestAsync(parsed, ct);

        if (snapshot is null)
            return NotFound();

        var response = new BlockchainSnapshotResponse
        {
            Id = snapshot.Id,
            BlockchainType = snapshot.BlockchainType.ToString(),
            SourceUrl = snapshot.SourceUrl,
            RawJson = snapshot.RawJson,
            CreatedAt = snapshot.CreatedAt,
            Height = snapshot.Height,
            Hash = snapshot.Hash,
            PeerCount = snapshot.PeerCount,
            UnconfirmedCount = snapshot.UnconfirmedCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets all available blockchain types.
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetBlockchainTypes()
    {
        var types = Enum.GetNames<BlockchainType>();
        return Ok(types);
    }
}
