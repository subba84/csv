using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

public class HelloEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HelloEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHello_NoLanguage_ReturnsEnglishGreeting()
    {
        var response = await _client.GetAsync("/api/hello");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ParseJson(response);
        Assert.Equal("Hello World", body);
    }

    [Theory]
    [InlineData("english",  "Hello World")]
    [InlineData("spanish",  "Hola Mundo")]
    [InlineData("french",   "Bonjour le Monde")]
    [InlineData("german",   "Hallo Welt")]
    [InlineData("japanese", "こんにちは世界")]
    public async Task GetHello_WithLanguage_ReturnsCorrectGreeting(string language, string expected)
    {
        var response = await _client.GetAsync($"/api/hello?language={language}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ParseJson(response);
        Assert.Equal(expected, body);
    }

    [Fact]
    public async Task GetHello_UnknownLanguage_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/hello?language=klingon");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("ENGLISH")]
    [InlineData("Spanish")]
    [InlineData("FRENCH")]
    public async Task GetHello_CaseInsensitiveLanguage_ReturnsOK(string language)
    {
        var response = await _client.GetAsync($"/api/hello?language={language}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<string?> ParseJson(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("message", out var msg) ? msg.GetString() : null;
    }
}
