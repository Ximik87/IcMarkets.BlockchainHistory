using IcMarkets.BlockchainHistory.Infrastructure;

namespace IcMarkets.BlockchainHistory.WorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddMediator();

        var host = builder.Build();


        host.Run();
    }
}