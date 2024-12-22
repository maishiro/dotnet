using GrafanaGraphqlService.DataCollector;
using GrafanaGraphqlService.Services;
using GrafanaGraphqlService.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<DataCollectorService>();
builder.Services.AddHostedService( sp => sp.GetRequiredService<DataCollectorService>() );

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.MapGraphQL();

app.Run();
