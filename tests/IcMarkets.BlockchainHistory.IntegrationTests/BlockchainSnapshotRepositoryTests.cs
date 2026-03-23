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

    [Fact]
    public async Task Add_ShouldPersistSnapshot_Test()
    {
        // Arrange
        var hash = Guid.NewGuid().ToString();
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC.main"}""",
            height: 800_000,
            hash: hash,
            peerCount: 250,
            unconfirmedCount: 1000,
            DateTimeOffset.UtcNow);

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
        persisted.Hash.ShouldBe(hash);
        persisted.PeerCount.ShouldBe(250);
        persisted.UnconfirmedCount.ShouldBe(1000);
    }

    [Fact]
    public async Task Update_ShouldModifyExistingSnapshot()
    {
        // Arrange
        var hash = Guid.NewGuid().ToString();
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC.main"}""",
            height: 800_000,
            hash: hash,
            peerCount: 250,
            unconfirmedCount: 1000,
            DateTimeOffset.UtcNow);
        _repository.Add(snapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        // Act
        snapshot.Update(snapshot.RawJson, createdAt: DateTimeOffset.UtcNow, height: 101, peerCount: 55,
            unconfirmedCount: 210);
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

    [Fact]
    public async Task GetHistoryAsync_ShouldFilterByBlockchainType()
    {
        // Arrange
        var date = DateTimeOffset.UtcNow.AddMinutes(-10);
        var btcSnapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC"}""",
            height: 1,
            hash: $"btc_{Guid.NewGuid()}",
            peerCount: 10,
            unconfirmedCount: 5,
            DateTimeOffset.UtcNow);
        var ethSnapshot = BlockchainSnapshot.Create(
            BlockchainType.Ethereum,
            """{"name":"ETH"}""",
            height: 2,
            hash: $"eth_{Guid.NewGuid()}",
            peerCount: 20,
            unconfirmedCount: 10,
            DateTimeOffset.UtcNow);

        _dbContext.BlockchainSnapshots.AddRange(btcSnapshot, ethSnapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);
        _dbContext.ChangeTracker.Clear();

        // Act
        var (btcHistory, _) =
            await _repository.GetHistoryAsync(BlockchainType.BitcoinMain, date, 1, 50, _cancellationToken);
        var (ethHistory, _) =
            await _repository.GetHistoryAsync(BlockchainType.Ethereum, date, 1, 50, _cancellationToken);

        // Assert
        btcHistory.Count.ShouldBeGreaterThanOrEqualTo(1);
        ethHistory.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnEmptyListWhenNoSnapshots()
    {
        // Act
        var (history, totalCount) =
            await _repository.GetHistoryAsync(BlockchainType.Dash, DateTimeOffset.UtcNow, 1, 50, _cancellationToken);

        // Assert
        history.ShouldBeEmpty();
        totalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetLatestAsync_ShouldReturnMostRecentSnapshot()
    {
        // Arrange
        var hash1 = $"dash_old_{Guid.NewGuid()}";
        var older = BlockchainSnapshot.Create(
            BlockchainType.Dash,
            """{"seq":1}""",
            height: 100,
            hash: $"dash_old_{Guid.NewGuid()}",
            peerCount: 5,
            unconfirmedCount: 3,
            DateTimeOffset.UtcNow.AddMinutes(-1));
        _dbContext.BlockchainSnapshots.Add(older);
        var newer = BlockchainSnapshot.Create(
            BlockchainType.Dash,
            """{"seq":2}""",
            height: 200,
            hash: hash1,
            peerCount: 10,
            unconfirmedCount: 6,
            DateTimeOffset.UtcNow);
        _dbContext.BlockchainSnapshots.Add(newer);
        await _dbContext.SaveChangesAsync(_cancellationToken);

        // Act
        var latest = await _repository.GetLatestAsync(BlockchainType.Dash, _cancellationToken);

        // Assert
        latest.ShouldNotBeNull();
        latest.Hash.ShouldBe(hash1);
        latest.Height.ShouldBe(200);
    }

    [Fact]
    public async Task GetLatestAsync_ShouldReturnNullWhenNoSnapshots()
    {
        // Act
        var latest = await _repository.GetLatestAsync(BlockchainType.BitcoinTest3, _cancellationToken);

        // Assert
        latest.ShouldBeNull();
    }

    [Fact]
    public async Task GetByHashAsync_ShouldReturnSnapshotWithMatchingHash()
    {
        // Arrange
        var hash = $"unique_hash_{Guid.NewGuid()}";
        var snapshot = BlockchainSnapshot.Create(
            BlockchainType.BitcoinMain,
            """{"name":"BTC"}""",
            height: 500,
            hash: hash,
            peerCount: 100,
            unconfirmedCount: 50,
            DateTimeOffset.UtcNow);
        _dbContext.BlockchainSnapshots.Add(snapshot);
        await _dbContext.SaveChangesAsync(_cancellationToken);


        // Act
        var result = await _repository.GetByHashAsync(hash, _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(snapshot.Id);
        result.Hash.ShouldBe(hash);
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
    public async ValueTask InitializeAsync()
    {
        await _dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}