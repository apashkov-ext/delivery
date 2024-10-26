using DeliveryApp.Core.Extensions;

namespace DeliveryApp.Api.Extensions;

internal static class RegisterServicesExtension
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.RegisterDomainServices();
        
        return builder;
    }
}