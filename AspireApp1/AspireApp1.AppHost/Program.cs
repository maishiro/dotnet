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
                    .WithEndpoint(port: 5437, targetPort: 5432); ;

var martdb = builder.AddContainer("martdb", "timescale/timescaledb", "latest-pg15")
                    .WithEnvironment("POSTGRES_DB", "martdb")
                    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                    .WithEndpoint(port: 5438, targetPort: 5432);

//var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");
var webapi = builder.AddExecutable( "webapi",
                                    "java",
                                    "D:\\work\\dotnet\\dotnet\\AspireApp1\\SpringBootWebAPI\\target",
                                    ["-javaagent:opentelemetry-javaagent.jar", "-Dotel.service.name=jaeger", "-jar", "demo-0.0.1-SNAPSHOT.jar"])
                    .WithHttpEndpoint(targetPort: 8080, name: "apiservice")
                    .WithOtlpExporter();
var apiservice = webapi.GetEndpoint("apiservice");

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiservice);



builder.Build().Run();
