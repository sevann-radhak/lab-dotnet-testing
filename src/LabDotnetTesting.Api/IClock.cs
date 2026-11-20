namespace LabDotnetTesting.Api;

public interface IClock
{
    DateTimeOffset Now { get; }
}

public sealed class FixedClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset Now { get; } = now;
}
