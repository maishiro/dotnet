using System.Text.Json;
using RagWeb.Models;

namespace RagWeb.Services;

public class VectorStore
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "Data", "vector_db.json");
    private readonly List<DocumentChunk> _chunks = new();
    private readonly object _lock = new();

    public VectorStore()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            var loaded = JsonSerializer.Deserialize<List<DocumentChunk>>(json);
            if (loaded != null) _chunks = loaded;
        }
    }

    public async Task AddAsync(IEnumerable<DocumentChunk> chunks, bool persist = true)
    {
        lock (_lock) _chunks.AddRange(chunks);
        if (persist) await SaveAsync();
    }

    public IReadOnlyList<DocumentChunk> All() => _chunks;

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_chunks);
        await File.WriteAllTextAsync(_path, json);
    }

    public List<DocumentChunk> Similar(float[] queryVec, int k = 3)
    {
        // コサイン類似度で上位 k
        var scored = _chunks
            .Select(c => (Chunk: c, Score: Cosine(queryVec, c.Vector)))
            .OrderByDescending(x => x.Score)
            .Take(k)
            .Select(x => x.Chunk)
            .ToList();
        return scored;
    }

    private static double Cosine(float[] a, float[] b)
    {
        if (a.Length != b.Length) return -1;
        double dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            na += a[i] * a[i];
            nb += b[i] * b[i];
        }
        if (na == 0 || nb == 0) return -1;
        return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
    }
}