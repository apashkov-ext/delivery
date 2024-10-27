using DeliveryApp.Core.Extensions;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Extensions;

namespace DeliveryApp.Api.Extensions;

internal static class RegisterServicesExtension
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.RegisterDomainServices();
        builder.Services.RegisterInfrastructureServices();
        
        builder.Services.AddOptions<DatabaseConfiguration>()
            .BindConfiguration(string.Empty)
            .ValidateDataAnnotations();
        
        return builder;
    }
}