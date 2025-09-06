using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RagWeb.Models;

namespace RagWeb.Services;

public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string EmbedModel { get; set; } = "mxbai-embed-large";
    public string ChatModel { get; set; } = "llama3.2:3b";
}

public class OllamaClient
{
    private readonly HttpClient _http;
    private readonly OllamaOptions _opt;

    public OllamaClient(IOptions<OllamaOptions> options)
    {
        _http = new HttpClient();
        _opt = options.Value;
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    {
        var req = new EmbedRequest { Model = _opt.EmbedModel, Input = text };
        var res = await _http.PostAsJsonAsync($"{_opt.BaseUrl}/api/embeddings", req, ct);
        res.EnsureSuccessStatusCode();
        var data = await res.Content.ReadFromJsonAsync<EmbedResponse>(cancellationToken: ct);
        return data?.Embedding ?? throw new InvalidOperationException("No embedding");
    }

    public async Task<string> ChatAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
    {
        var req = new ChatRequest
        {
            Model = _opt.ChatModel,
            Messages = new()
            {
                new ChatMessage { Role = "system", Content = systemPrompt },
                new ChatMessage { Role = "user", Content = userPrompt }
            },
            Stream = false,
            Options = new ChatOptions { Temperature = 0.0 }
        };

        var res = await _http.PostAsJsonAsync($"{_opt.BaseUrl}/api/chat", req, ct);
        res.EnsureSuccessStatusCode();
        var data = await res.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: ct);
        return data?.Message?.Content ?? "";
    }
}