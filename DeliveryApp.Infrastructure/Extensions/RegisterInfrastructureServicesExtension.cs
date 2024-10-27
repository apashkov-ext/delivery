using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Primitives;

namespace DeliveryApp.Infrastructure.Extensions;

public static class RegisterInfrastructureServicesExtension
{
    public static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services.RegisterRepositories();
        services.RegisterDbContext();
    }
    
    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICourierRepository, CourierRepository>();
    }

    private static void RegisterDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((prov, optionsBuilder) =>
        {
            var config = prov.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            var connStr = config.CONNECTION_STRING;
            
            optionsBuilder.UseNpgsql(connStr, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
            });
            optionsBuilder.EnableSensitiveDataLogging();
        });
    }
}