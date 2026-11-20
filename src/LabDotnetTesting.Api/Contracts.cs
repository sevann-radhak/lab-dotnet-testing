using System.ComponentModel;

namespace LabDotnetTesting.Api;

public sealed record OrderRequest(
    [property: Description("Customer name. Required.")]
    string? Customer,
    [property: Description("Amount before the discount.")]
    decimal Amount);

public sealed record OrderResponse(
    [property: Description("Amount after the discount rule. Ten percent off when the clock is before noon.")]
    decimal Total);
