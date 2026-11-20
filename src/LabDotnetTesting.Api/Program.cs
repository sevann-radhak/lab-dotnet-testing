using LabDotnetTesting.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton<DiscountRule>();

string clockMode = builder.Configuration["Lab:Clock"] ?? "fixed";
if (clockMode != "missing")
{
    _ = builder.Services.AddSingleton<IClock>(new FixedClock(new DateTimeOffset(2026, 6, 1, 11, 0, 0, TimeSpan.Zero)));
}

builder.Services.AddOpenApi(options =>
{
    _ = options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "lab-dotnet-testing";
        document.Info.Version = "v1";
        document.Info.Description =
            """
            A discount rule and the HTTP host around it.

            The host clock is 11:00, so a valid order is 10 percent off. DiscountTests covers the rule with a clock double and does not open a port. OrderEndpointTests sends a real request through the host.

            POST /orders without a customer is 400. A host built without the clock returns 500. The unit tests stay green either way.
            """;
        return Task.CompletedTask;
    });
});

WebApplication app = builder.Build();

_ = app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
    _ = app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "lab-dotnet-testing v1");
        options.DocumentTitle = "lab-dotnet-testing";
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth")
    .WithTags("Health")
    .WithSummary("Liveness probe")
    .WithDescription("Returns 200 when the process can serve HTTP.")
    .AllowAnonymous();

app.MapPost("/orders", (OrderRequest? request, IClock clock, DiscountRule rule) =>
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Customer))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["customer"] = ["Customer is required."],
            });
        }

        if (request.Amount <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["amount"] = ["Amount must be greater than zero."],
            });
        }

        decimal total = rule.Total(request.Amount, clock);
        return Results.Ok(new OrderResponse(total));
    })
    .WithName("CreateOrder")
    .WithTags("Orders")
    .WithSummary("Price an order with the discount rule")
    .WithDescription(
        "Customer is required. The host clock is 11:00, so the total is 10 percent below the amount. " +
        "An empty customer is 400. A host that did not register the clock fails with 500 before the rule runs.")
    .Produces<OrderResponse>(StatusCodes.Status200OK)
    .ProducesValidationProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status500InternalServerError);

app.Run();

public partial class Program;
