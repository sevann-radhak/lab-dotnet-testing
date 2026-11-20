namespace LabDotnetTesting.Api;

public sealed class DiscountRule
{
    public decimal Total(decimal amount, IClock clock)
    {
        return clock.Now.Hour < 12 ? decimal.Round(amount * 0.9m, 2, MidpointRounding.AwayFromZero) : amount;
    }
}
