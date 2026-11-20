using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LabDotnetTesting.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>;

public sealed class MissingClockApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.UseSetting("Lab:Clock", "missing");
    }
}
