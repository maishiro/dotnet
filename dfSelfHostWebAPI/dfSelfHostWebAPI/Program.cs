using dfSelfHostWebAPI.Services;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.SelfHost;


namespace dfSelfHostWebAPI
{
    internal class Program
    {
        private const int DefaultPort = 9000;

        static void Main( string [] args )
        {
            log4net.Config.XmlConfigurator.Configure();

            var port = DefaultPort;
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            {
                port = customPort;
            }

            var baseUrl = $"http://localhost:{port}";
            var config = new HttpSelfHostConfiguration(baseUrl);

            // Web API ルート設定
            config.MapHttpAttributeRoutes();

            // Simple Injector 設定
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            // オープンジェネリック登録を使用
            container.Register(typeof(ILogger<>), typeof(Log4NetLogger<>), Lifestyle.Singleton);

            container.Register<IMyService, MyService>( Lifestyle.Scoped );
            container.RegisterWebApiControllers( config );
            container.Verify();

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver( container );

            // Swagger 有効化
            config.EnableSwagger( c => {
                c.SingleApiVersion("v1", "SelfHost API");

                // 実行時の出力フォルダ（通常は bin\Debug\...）にある XML コメントを読み込む
                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            } )
            .EnableSwaggerUi();

            // サーバ起動
            using( var server = new HttpSelfHostServer( config ) )
            {
                server.OpenAsync().Wait();
                Console.WriteLine( $"SelfHost running at {baseUrl}" );
                Console.ReadLine();
            }

        }
    }
}
