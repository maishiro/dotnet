namespace RagWeb.Models;

public class DocumentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Source { get; set; } = "upload";
    public string Content { get; set; } = "";
    public float[] Vector { get; set; } = Array.Empty<float>();
}
