namespace RagWeb.Services;

public class TextChunker
{
    public IEnumerable<string> Split(string text, int chunkSize = 500, int overlap = 50)
    {
        text = text.Replace("\r\n", "\n");
        var start = 0;
        while (start < text.Length)
        {
            var end = Math.Min(start + chunkSize, text.Length);
            yield return text[start..end];
            if (end == text.Length) break;
            start = Math.Max(0, end - overlap);
        }
    }
}
