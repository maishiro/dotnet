using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RagWeb.Models;
using RagWeb.Services;

namespace RagWeb.Pages;

public class IndexModel : PageModel
{
    private readonly OllamaClient _ollama;
    private readonly VectorStore _store;
    private readonly TextChunker _chunker;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(OllamaClient ollama, VectorStore store, TextChunker chunker, ILogger<IndexModel> logger)
    {
        _ollama = ollama;
        _store = store;
        _chunker = chunker;
        _logger = logger;
    }

    [BindProperty]
    public string? Query { get; set; }

    public string? UploadStatus { get; set; }
    public string? Answer { get; set; }
    public List<DocumentChunk>? ContextChunks { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostUploadAsync(IFormFile? upload, CancellationToken ct)
    {
        if (upload == null || upload.Length == 0)
        {
            UploadStatus = "ファイルが選択されていません。";
            return Page();
        }

        using var reader = new StreamReader(upload.OpenReadStream());
        var text = await reader.ReadToEndAsync();

        var chunks = new List<DocumentChunk>();
        foreach (var part in _chunker.Split(text, chunkSize: 500, overlap: 50))
        {
            var vec = await _ollama.EmbedAsync(part, ct);
            chunks.Add(new DocumentChunk { Source = upload.FileName, Content = part, Vector = vec });
        }

        await _store.AddAsync(chunks);
        UploadStatus = $"ドキュメント {upload.FileName} を {chunks.Count} チャンクで追加しました。";
        return Page();
    }

    public async Task<IActionResult> OnPostAskAsync(string? query, CancellationToken ct)
    {
        Query = query?.Trim();
        if (string.IsNullOrWhiteSpace(Query))
        {
            Answer = "質問が空です。";
            return Page();
        }

        var qvec = await _ollama.EmbedAsync(Query, ct);
        var top = _store.Similar(qvec, k: 3);
        ContextChunks = top;

        var context = string.Join("\n---\n", top.Select(t => t.Content));
        var system = "あなたは厳密で正確なアシスタントです。以下のコンテキストのみを根拠に日本語で簡潔に回答し、不明な場合は「わかりません」と答えてください。";
        var user = $"コンテキスト:\n{context}\n\n質問: {Query}";

        Answer = await _ollama.ChatAsync(system, user, ct);
        return Page();
    }
}