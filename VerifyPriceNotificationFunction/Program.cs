using EstevesPriceAlert.Application;
using EstevesPriceAlert.InfraEstructure;
using EstevesPriceAlert.InfraEstructure.Persistence; // MongoDbOptions
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// builder.ConfigureFunctionsWorkerDefaults(w => w.AddTimers());
builder.ConfigureFunctionsWebApplication();

// Monte o options a partir das env vars (Values -> variáveis com "__")
var mongoOptions = new MongoDbOptions
{
    ConnectionString = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "",
    Database = Environment.GetEnvironmentVariable("Mongo__Database") ?? ""
};
builder.Services.AddSingleton(mongoOptions);

builder.Services
    .AddHandlers()
    .AddRepositories()
    .AddMongo();

builder.Services.AddHttpClient();

builder.Build().Run();
