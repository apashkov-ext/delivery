using DeliveryApp.Api;
using DeliveryApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
        });
});

// Configuration
// services.Configure<Settings>(options => Configuration.Bind(options));
// var connectionString = Configuration["CONNECTION_STRING"];
// var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
// var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];

builder.Services.AddOptions<Settings>()
    .BindConfiguration(string.Empty)
    .ValidateDataAnnotations();


builder.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseRouting();

app.Run();