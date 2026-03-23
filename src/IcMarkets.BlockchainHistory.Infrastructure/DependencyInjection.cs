using FluentValidation;
using IcMarkets.BlockchainHistory.Application.Abstractions.External;
using IcMarkets.BlockchainHistory.Application.Abstractions.Persistence;
using IcMarkets.BlockchainHistory.Application.Behaviors;
using IcMarkets.BlockchainHistory.Application.Features.Blockchain;
using IcMarkets.BlockchainHistory.Infrastructure.External;
using IcMarkets.BlockchainHistory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IcMarkets.BlockchainHistory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IBlockchainSnapshotRepository, BlockchainSnapshotRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.Configure<BlockCypherOptions>(configuration.GetSection("BlockCypher"));
        services.AddHttpClient();
        services.AddTransient<IBlockCypherClient, BlockCypherClient>();
        services.AddMediator(opt => opt.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddTransient(typeof(Mediator.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssemblyContaining<IBlockchainSnapshotSynchronizer>();
        services.AddTransient<IBlockchainSnapshotSynchronizer, BlockchainSnapshotSynchronizer>();

        return services;
    }
}