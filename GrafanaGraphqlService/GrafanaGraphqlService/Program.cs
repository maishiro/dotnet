using GrafanaGraphqlService.DataCollector;
using GrafanaGraphqlService.Types;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddSingleton<DataCollectorService>();
// builder.Services.AddHostedService<DataCollectorService>();
builder.Host.UseWindowsService();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<DataCollectorService>();
builder.Services.AddHostedService( sp => sp.GetRequiredService<DataCollectorService>() );

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.MapGraphQL();

app.Run();
