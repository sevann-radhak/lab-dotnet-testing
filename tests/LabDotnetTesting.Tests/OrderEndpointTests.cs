using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LabDotnetTesting.Tests;

public sealed class OrderEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task MissingCustomerIs400()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/orders", new { amount = 100 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PricedOrder()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/orders", new { customer = "Ada", amount = 100 });
        _ = response.EnsureSuccessStatusCode();
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(90m, document.RootElement.GetProperty("total").GetDecimal());
    }

    [Fact]
    public async Task MissingClock()
    {
        await using MissingClockApiFactory missing = new();
        HttpClient client = missing.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/orders", new { customer = "Ada", amount = 100 });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_ui_is_available_in_development()
    {
        HttpResponseMessage response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
