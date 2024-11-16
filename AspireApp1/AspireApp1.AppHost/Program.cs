using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


var broker = builder.AddRabbitMQ("broker")
                    .WithManagementPlugin()
                    .WithImage("rabbitmq", "3-management")
                    .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
                    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "admin")
                    .WithHttpEndpoint(port: 15672, targetPort: 15672);

var lakedb = builder.AddPostgres("lakedb")
                    .WithImage("timescale/timescaledb", "latest-pg15")
                    .WithEnvironment("POSTGRES_DB", "lakedb")
                    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                    .WithEndpoint( port: 5437, targetPort: 5432, scheme: "tcp", name:"lakedb" );


var martdb = builder.AddContainer("martdb", "timescale/timescaledb", "latest-pg15")
                    .WithEnvironment("POSTGRES_DB", "martdb")
                    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                    .WithEndpoint( port: 5438, targetPort: 5432, name:"martdb" );

var apiservice = builder.AddSpringApp(
					    "webapi",
                        workingDirectory: "../SpringBootWebAPI/target/",
                        new JavaAppExecutableResourceOptions()
                        {
                            ApplicationName = "demo-0.0.1-SNAPSHOT.jar",
                            Port = 8080,
                            OtelAgentPath = "./",
                        })
                        .WithHttpEndpoint( port:8080, targetPort: 8080, name:"webapi", isProxied: false )
                        .PublishAsDockerFile(
                        [
                            new DockerBuildArg( "JAR_NAME", "demo-0.0.1-SNAPSHOT.jar" ),
                            new DockerBuildArg( "AGENT_PATH", "/" ),
                            new DockerBuildArg( "SERVER_PORT", "8080" ),
                        ]);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiservice);



builder.Build().Run();
