using IcMarkets.BlockchainHistory.Infrastructure;

namespace IcMarkets.BlockchainHistory.WorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddMediator(opt => opt.ServiceLifetime = ServiceLifetime.Scoped);

        var host = builder.Build();


        host.Run();
    }
}