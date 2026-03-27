var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5050");

var app = builder.Build();

var greetings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { "english",  "Hello World" },
    { "spanish",  "Hola Mundo" },
    { "french",   "Bonjour le Monde" },
    { "german",   "Hallo Welt" },
    { "japanese", "こんにちは世界" }
};

app.MapGet("/api/hello", (string? language) =>
{
    var lang = language ?? "english";
    if (!greetings.TryGetValue(lang, out var message))
        return Results.BadRequest(new { error = $"Unknown language: {lang}" });

    return Results.Ok(new { message });
});

app.Run();

public partial class Program { }
