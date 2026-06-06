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

var apiservice = builder.AddExecutable(
					    name: "webapi",
                        command: @"java.exe",
                        args: new[]
                        {
                            "-jar",
                            "demo-0.0.1-SNAPSHOT.jar",
                            "--server.port=8080"
                        },
                        workingDirectory: "../SpringBootWebAPI/target"
                    )
                    .WithReference(lakedb)
                    .WithReference(martdb)
                    .WaitFor(lakedb)
                    .WaitFor(martdb)
                    .WithHttpEndpoint( name:"http", port:8080, targetPort: 8080, isProxied: false );

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(broker);



builder.Build().Run();
