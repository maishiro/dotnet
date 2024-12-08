using GrafanaGrpcService.Server.DataCollector;
using GrafanaGrpcService.Server.Services;
using Microsoft.Extensions.Hosting.WindowsServices;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
});

builder.Host.UseWindowsService();

builder.Services.AddGrpc();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DataCollectorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DataCollectorService>());

var app = builder.Build();

app.MapGrpcService<GrafanaQueryService>();

app.Run();
