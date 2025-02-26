using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureFunctionsWorkerDefaults()
//    .Build();

builder.ConfigureFunctionsWebApplication();

IHost host = builder.Build();
await host.RunAsync();
