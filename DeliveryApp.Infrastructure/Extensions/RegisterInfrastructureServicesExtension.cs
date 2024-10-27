using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Infrastructure.Extensions;

public static class RegisterInfrastructureServicesExtension
{
    public static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services.RegisterRepositories();
    }
    
    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICourierRepository, CourierRepository>();
    }
}