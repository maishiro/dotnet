var builder = DistributedApplication.CreateBuilder(args);



var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);
var broker = builder.AddRabbitMQ("broker", username, password)
                    .WithManagementPlugin();

var lakeUsername = builder.AddParameter("lakeuser", secret: true);
var lakePassword = builder.AddParameter("lakepass", secret: true);
var lakedb = builder.AddPostgres("lake", lakeUsername, lakePassword)
                    .WithImage("timescale/timescaledb", "latest-pg15")
                    .AddDatabase("lakedb");
//var lake = builder.AddContainer("lakedb", "timescale/timescaledb", "latest-pg15")
//                    .WithEnvironment("POSTGRES_DB", "lakedb")
//                    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
//                    .WithEndpoint( port: 5437, targetPort: 5432, name:"lakedb" );
//var lakedb = lake.GetEndpoint("lakedb");

var martUsername = builder.AddParameter("martuser", secret: true);
var martPassword = builder.AddParameter("martpass", secret: true);
var martdb = builder.AddPostgres("mart", martUsername, martPassword)
                    .WithImage("timescale/timescaledb", "latest-pg15")
                    .AddDatabase("martdb");
//var mart = builder.AddContainer("martdb", "timescale/timescaledb", "latest-pg15")
//                    .WithEnvironment("POSTGRES_DB", "martdb")
//                    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
//                    .WithEndpoint( port: 5438, targetPort: 5432, name:"martdb" );
//var martdb = mart.GetEndpoint("martdb");

var apiservice = builder.AddSpringApp(
					    "webapi",
                        workingDirectory: "../SpringBootWebAPI/target/",
                        new JavaAppExecutableResourceOptions()
                        {
                            ApplicationName = "demo-0.0.1-SNAPSHOT.jar",
                            Port = 8080,
                            OtelAgentPath = "./",
                        })
                        .WithReference(lakedb)
                        .WithReference(martdb)
                        .WithHttpEndpoint( port:8080, targetPort: 8080, name:"webapi", isProxied: false )
                        .PublishAsDockerFile(
                        [
                            new DockerBuildArg( "JAR_NAME", "demo-0.0.1-SNAPSHOT.jar" ),
                            new DockerBuildArg( "AGENT_PATH", "/" ),
                            new DockerBuildArg( "SERVER_PORT", "8080" ),
                        ]);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(broker)
    .WithReference(apiservice);



builder.Build().Run();
