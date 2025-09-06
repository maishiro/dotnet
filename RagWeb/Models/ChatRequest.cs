namespace RagWeb.Models;

public class ChatRequest
{
    public string Model { get; set; } = "";
    public List<ChatMessage> Messages { get; set; } = new();
    public bool Stream { get; set; } = false;
    public ChatOptions? Options { get; set; }
}

public class ChatMessage
{
    public string Role { get; set; } = "user"; // system|user|assistant
    public string Content { get; set; } = "";
}

public class ChatOptions
{
    public double Temperature { get; set; } = 0.0;
}
