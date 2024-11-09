using Microsoft.AspNetCore.HttpLogging;
using WebSocketService;

namespace WebSocketWindowsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors( options =>
            {
                options.AddPolicy( "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    } );
            } );

            services.AddControllers();
            services.AddSingleton<WebSocketHandler>();

            services.AddHttpLogging( logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add( "Content-Type" );
                logging.MediaTypeOptions.AddText( "application/json" );
            } );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpLogging();
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseRouting();

            app.UseCors( "AllowAll" );

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/connect", async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var handler = app.ApplicationServices.GetRequiredService<WebSocketHandler>();
                        await handler.HandleWebSocketConnection(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                });
            });
        }
    }
}