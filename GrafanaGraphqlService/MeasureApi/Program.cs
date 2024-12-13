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

app.MapGet("/temperature", () =>
{
    var measure =
        new MeasureTemperature
        (
            // DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            DateTimeOffset.Now,
            Random.Shared.Next(-20, 55)
        );
    return measure;
})
.WithName("GetMeasureTemperature")
.WithOpenApi();

app.Run();

record MeasureTemperature(DateTimeOffset DateTime, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
