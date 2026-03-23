using IcMarkets.BlockchainHistory.Domain.Entities;
using IcMarkets.BlockchainHistory.Domain.Enums;
using IcMarkets.BlockchainHistory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace IcMarkets.BlockchainHistory.IntegrationTests;

public sealed class BlockchainSnapshotRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly BlockchainSnapshotRepository _repository;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public BlockchainSnapshotRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Postgres");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new BlockchainSnapshotRepository(_dbContext);
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task Add_ShouldPersistSnapshot_Test()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC.main"}""",
            height: 800_000,
            hash: "abc123",
            peerCount: 250,
            unconfirmedCount: 1000);

        // Act
        _repository.Add(snapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        // Assert
        var persisted = await _dbContext.BlockchainSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == snapshot.Id, _cancellationToken);
        persisted.ShouldNotBeNull();
        persisted.BlockchainType.ShouldBe(BlockchainType.BitcoinMain);
        persisted.RawJson.ShouldBe("""{"name": "BTC.main"}""");
        persisted.Height.ShouldBe(800_000);
        persisted.Hash.ShouldBe("abc123");
        persisted.PeerCount.ShouldBe(250);
        persisted.UnconfirmedCount.ShouldBe(1000);
    }

    [Fact]
    public async Task Update_ShouldModifyExistingSnapshot()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC.main"}""",
            height: 800_000,
            hash: "abc123",
            peerCount: 250,
            unconfirmedCount: 1000);
        _repository.Add(snapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        // Act
        snapshot.Update(snapshot.RawJson, height: 101, peerCount: 55, unconfirmedCount: 210);
        _repository.Update(snapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        // Assert
        var updated = await _dbContext.BlockchainSnapshots
            .AsNoTracking()
            .FirstAsync(s => s.Id == snapshot.Id, _cancellationToken);
        updated.Height.ShouldBe(101);
        updated.PeerCount.ShouldBe(55);
        updated.UnconfirmedCount.ShouldBe(210);
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistoryAsync_ShouldReturnSnapshotsOrderedByCreatedAtDescending()
    {
        // Arrange
        var snapshot1 = BlockchainSnapshot.Create(
            BlockchainType.Litecoin,
            """{"seq":1}""",
            height: 1,
            hash: "ltc_hash_1",
            peerCount: 10,
            unconfirmedCount: 5);

        _dbContext.BlockchainSnapshots.Add(snapshot1);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        await Task.Delay(50); // ensure different CreatedAt

        var snapshot2 = BlockchainSnapshot.Create(
            BlockchainType.Litecoin,
            """{"seq":2}""",
            height: 2,
            hash: "ltc_hash_2",
            peerCount: 20,
            unconfirmedCount: 10);

        _dbContext.BlockchainSnapshots.Add(snapshot2);
        await _dbContext.SaveChangesAsync(_cancellationToken);
        _dbContext.ChangeTracker.Clear();

        // Act
        var history = await _repository.GetHistoryAsync(BlockchainType.Litecoin, DateTimeOffset.UtcNow, _cancellationToken);

        // Assert
        history.Count.ShouldBe(2);
        history[0].Hash.ShouldBe("ltc_hash_2");
        history[1].Hash.ShouldBe("ltc_hash_1");
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistoryAsync_ShouldFilterByBlockchainType()
    {
        // Arrange
        var btcSnapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC"}""",
            height: 1,
            hash: "btc_filter_hash",
            peerCount: 10,
            unconfirmedCount: 5);

        var ethSnapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum,
            """{"name":"ETH"}""",
            height: 2,
            hash: "eth_filter_hash",
            peerCount: 20,
            unconfirmedCount: 10);

        _dbContext.BlockchainSnapshots.AddRange(btcSnapshot, ethSnapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);
        _dbContext.ChangeTracker.Clear();

        // Act
        var btcHistory = await _repository.GetHistoryAsync(BlockchainType.BitcoinMain, DateTimeOffset.UtcNow, _cancellationToken);
        var ethHistory = await _repository.GetHistoryAsync(BlockchainType.Ethereum, DateTimeOffset.UtcNow, _cancellationToken);

        // Assert
        btcHistory.Count.ShouldBe(1);
        btcHistory[0].Hash.ShouldBe("btc_filter_hash");

        ethHistory.Count.ShouldBe(1);
        ethHistory[0].Hash.ShouldBe("eth_filter_hash");
    }

    [Fact(Skip = "fix later")]
    public async Task GetHistoryAsync_ShouldReturnEmptyListWhenNoSnapshots()
    {
        // Act
        var history = await _repository.GetHistoryAsync(BlockchainType.Dash, DateTimeOffset.UtcNow, _cancellationToken);

        // Assert
        history.ShouldBeEmpty();
    }

    [Fact(Skip = "fix later")]
    public async Task GetLatestAsync_ShouldReturnMostRecentSnapshot()
    {
        // Arrange
        var older = BlockchainSnapshot.Create(
            BlockchainType.Dash,
            """{"seq":1}""",
            height: 100,
            hash: "dash_old",
            peerCount: 5,
            unconfirmedCount: 3);

        _dbContext.BlockchainSnapshots.Add(older);
        await _dbContext.SaveChangesAsync();

        await Task.Delay(50);

        var newer = BlockchainSnapshot.Create(
            BlockchainType.Dash,
            """{"seq":2}""",
            height: 200,
            hash: "dash_new",
            peerCount: 10,
            unconfirmedCount: 6);

        _dbContext.BlockchainSnapshots.Add(newer);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        var latest = await _repository.GetLatestAsync(BlockchainType.Dash, _cancellationToken);

        // Assert
        latest.ShouldNotBeNull();
        latest.Hash.ShouldBe("dash_new");
        latest.Height.ShouldBe(200);
    }

    [Fact(Skip = "fix later")]
    public async Task GetLatestAsync_ShouldReturnNullWhenNoSnapshots()
    {
        // Act
        var latest = await _repository.GetLatestAsync(BlockchainType.BitcoinTest3, _cancellationToken);

        // Assert
        latest.ShouldBeNull();
    }

    [Fact(Skip = "fix later")]
    public async Task GetByHashAsync_ShouldReturnSnapshotWithMatchingHash()
    {
        // Arrange
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC"}""",
            height: 500,
            hash: "unique_hash_123",
            peerCount: 100,
            unconfirmedCount: 50);

        _dbContext.BlockchainSnapshots.Add(snapshot);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByHashAsync("unique_hash_123", _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(snapshot.Id);
        result.Hash.ShouldBe("unique_hash_123");
        result.Height.ShouldBe(500);
    }

    [Fact]
    public async Task GetByHashAsync_ShouldReturnNullWhenHashNotFound()
    {
        // Act
        var result = await _repository.GetByHashAsync("nonexistent_hash", _cancellationToken);

        // Assert
        result.ShouldBeNull();
    }
}