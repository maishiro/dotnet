using RagWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// 設定
builder.Services.Configure<OllamaOptions>(opt =>
{
    opt.BaseUrl = "http://localhost:11434";
    opt.EmbedModel = "mxbai-embed-large";
    opt.ChatModel = "llama3.2:3b";
});

// DI
builder.Services.AddSingleton<OllamaClient>();
builder.Services.AddSingleton<VectorStore>();
builder.Services.AddSingleton<TextChunker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();