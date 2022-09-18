using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo
{
    public class UserName
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenPayload
    {
        public string Token { get; set; }
    }

    public class Pet
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddGraphQLServer()
                .AddDocumentFromFile( "./schema.graphql" )
                // 
                .BindRuntimeType<Pet>()
                .AddResolver("Query", "pet", (context) =>
                {
                    // HTTP Header - Authorization - token
                    var token = string.Empty;
                    var httpContext = context.ContextData["HttpContext"] as DefaultHttpContext;
                    if( httpContext != null ) 
                    {
                        if( httpContext.Request.Headers.ContainsKey( "Authorization" ) )
                        {
                            var items = httpContext.Request.Headers["Authorization"];
                            foreach( var item in items )
                            {
                                if( !string.IsNullOrEmpty(item) )
                                {
                                    token = item.Substring( "token ".Length );
                                    break;
                                }
                            }
                        }
                    }

                    var id = context.ArgumentValue<string>("id");
                    return new Pet{ Id = id, Name = "kijitora" };
                })
                // 
                .BindRuntimeType<UserName>()
                .BindRuntimeType<TokenPayload>()
                .AddResolver( "Mutation", "login", (context) =>
                {
                    var input = context.ArgumentValue<UserName>("input");
                    return new TokenPayload{ Token = "1234567890" };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapGraphQL();
            } );
        }
    }
}
