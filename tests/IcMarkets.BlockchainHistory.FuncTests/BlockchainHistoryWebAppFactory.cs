using IcMarkets.BlockchainHistory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IcMarkets.BlockchainHistory.FuncTests;

public sealed class BlockchainHistoryWebAppFactory : WebApplicationFactory<Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            // Add in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("FuncTestsDb_" + Guid.NewGuid()));
        });

        builder.UseEnvironment("Development");
    }
}
