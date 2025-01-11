var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/temperature", () =>
{
    var measures =  Enumerable.Range(1, 5).Select(index =>
        new MeasureTemperature
        (
            //DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            DateTimeOffset.Now,
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return measures;
})
.WithName("GetMeasureTemperature")
.WithOpenApi();

app.Run();

record MeasureTemperature( DateTimeOffset DateTime, int TemperatureC, string? Summary )
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
