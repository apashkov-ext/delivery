using DeliveryApp.Core.Application.UseCases.Queries;
using DeliveryApp.Core.Extensions;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Api.Extensions;

internal static class RegisterServicesExtension
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.RegisterDomainServices();
        builder.Services.RegisterCommandsAndQueries();
        builder.Services.RegisterInfrastructureServices();
        
        builder.Services.AddOptions<DatabaseConfiguration>()
            .BindConfiguration(string.Empty)
            .ValidateDataAnnotations();        
        
        builder.Services.AddOptions<GeoServiceConfiguration>()
            .BindConfiguration(string.Empty)
            .ValidateDataAnnotations();

        builder.Services.AddSingleton(prov =>
        {
            var config = prov.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            return new PostgresConnectionString(config.CONNECTION_STRING);
        });
        
        return builder;
    }
}