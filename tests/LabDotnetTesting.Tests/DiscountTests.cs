using LabDotnetTesting.Api;
using Moq;

namespace LabDotnetTesting.Tests;

public sealed class DiscountTests
{
    private readonly DiscountRule _rule = new();

    [Fact]
    public void AppliesTenPercentBeforeNoon()
    {
        Mock<IClock> clock = new();
        _ = clock.Setup(item => item.Now).Returns(At(11));

        decimal total = _rule.Total(100m, clock.Object);

        Assert.Equal(90m, total);
    }

    [Fact]
    public void NoDiscountAfterNoon()
    {
        Mock<IClock> clock = new();
        _ = clock.Setup(item => item.Now).Returns(At(13));

        decimal total = _rule.Total(100m, clock.Object);

        Assert.Equal(100m, total);
    }

    private static DateTimeOffset At(int hour)
    {
        return new DateTimeOffset(2026, 6, 1, hour, 0, 0, TimeSpan.Zero);
    }
}
