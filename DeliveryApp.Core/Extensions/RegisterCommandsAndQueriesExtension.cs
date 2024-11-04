using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Core.Extensions;

public static class RegisterCommandsAndQueriesExtension
{
    public static void RegisterCommandsAndQueries(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}