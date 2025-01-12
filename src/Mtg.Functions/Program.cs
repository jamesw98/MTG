using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mtg.Functions.Utils;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register services
builder.Services.AddSingleton<MongoUtil>();
builder.Services.AddSingleton<AuthUtil>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7085") // Correct Blazor origin
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Build().Run();