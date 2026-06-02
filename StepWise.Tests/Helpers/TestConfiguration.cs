using Microsoft.Extensions.Configuration;

namespace StepWise.Tests.Helpers
{
    public static class TestConfiguration
    {
        public static IConfiguration GetConfiguration()
        {
            var settings = new Dictionary<string, string>
            {
                { "Jwt:Secret", "StepWise@ChaveSecreta2026!XyZ#123456789" },
                { "Jwt:ExpiresInHours", "8" }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();
        }
    }
}
