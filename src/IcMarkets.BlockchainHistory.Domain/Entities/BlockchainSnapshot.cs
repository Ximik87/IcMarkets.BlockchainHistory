using System;
using System.Collections.Generic;
using System.Text;
using IcMarkets.BlockchainHistory.Domain.Enums;

namespace IcMarkets.BlockchainHistory.Domain.Entities;

public sealed class BlockchainSnapshot
{
    public Guid Id { get; private set; }
    public BlockchainType BlockchainType { get; private set; }   // Eth, Dash, BtcMain, BtcTest3, Ltc
    public string SourceUrl { get; private set; } = default!;
    public string RawJson { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    // optional normalized fields for quick filtering/display
    public long? Height { get; private set; }
    public string? Hash { get; private set; }
    public int? PeerCount { get; private set; }
    public int? UnconfirmedCount { get; private set; }
}