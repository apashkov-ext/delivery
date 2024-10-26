using DeliveryApp.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Core.Extensions;

public static class RegisterDomainServicesExtension
{
    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IDispatchService, DispatchService>();
    }
}