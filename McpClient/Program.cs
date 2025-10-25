using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "Everything",
    Command = "npx",
    Arguments = ["-y", "@modelcontextprotocol/server-everything"],
});

var client = await McpClient.CreateAsync(clientTransport);

// Print the list of tools available from the server.
Console.WriteLine( "-- Tools --" );
foreach (var tool in await client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

Console.WriteLine( "-- Prompts --" );
foreach (var prompt in await client.ListPromptsAsync())
{
    Console.WriteLine($"{prompt.Name} ({prompt.Description})");
}

Console.WriteLine( "-- Resources --" );
foreach (var resource in await client.ListResourcesAsync())
{
    Console.WriteLine($"{resource.Name} ({resource.MimeType}) [{resource.Uri}]");
}

Console.WriteLine( "-- ResourceTemplates --" );
foreach (var resourceTemplate in await client.ListResourceTemplatesAsync())
{
    Console.WriteLine($"{resourceTemplate.Name} ({resourceTemplate.Description}) [{resourceTemplate.UriTemplate}] ");
}


// Execute a tool (this would normally be driven by LLM tool invocations).
var result = await client.CallToolAsync(
    "echo",
    new Dictionary<string, object?>() { ["message"] = "Hello MCP!" },
    cancellationToken:CancellationToken.None);

// echo always returns one and only one text content object
Console.WriteLine( ((TextContentBlock)result.Content.First(c => c.Type == "text"))?.Text );
