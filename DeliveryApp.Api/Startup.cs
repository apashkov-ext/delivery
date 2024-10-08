namespace DeliveryApp.Api;

// Этот шаблон безнадежно устарел, он затрудняет настройку приложения.
// https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-8.0&tabs=visual-studio
// дотнет здесь восьмой, а шаблон из пятого. Исправьте уже, а то уж очень несовременно.
public class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        Configuration = configuration;
    }

    /// <summary>
    ///     Конфигурация
    /// </summary>
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Health Checks
        services.AddHealthChecks();

        // Cors
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin(); // Не делайте так в проде!
                });
        });

        // Configuration
        services.Configure<Settings>(options => Configuration.Bind(options));
        var connectionString = Configuration["CONNECTION_STRING"];
        var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
        var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseHealthChecks("/health");
        app.UseRouting();
    }
}