using IcMarkets.BlockchainHistory.Infrastructure;

namespace IcMarkets.BlockchainHistory.WorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddMemoryCache();
        builder.Services.AddHostedService<BlockchainDataPollingService>();

        builder.Services.AddHealthChecks()
            .AddNpgSql(builder.Configuration.GetConnectionString("Postgres") ?? string.Empty);

        var app = builder.Build();

        app.MapHealthChecks("/health");
       
        app.Run();
    }
}