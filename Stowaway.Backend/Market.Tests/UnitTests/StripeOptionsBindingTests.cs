using Market.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Market.Tests.UnitTests;

public class StripeOptionsBindingTests
{
    [Fact]
    public void Configure_BindsWebhookSecret_FromStripeSection()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Stripe:ApiKey"] = "sk_test_123",
                ["Stripe:WebhookSecret"] = "whsec_test_456",
                ["Stripe:Currency"] = "usd"
            })
            .Build();

        var services = new ServiceCollection();
        services.Configure<StripeOptions>(config.GetSection("Stripe"));
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<StripeOptions>>().Value;

        Assert.Equal("sk_test_123", options.ApiKey);
        Assert.Equal("whsec_test_456", options.WebhookSecret);
        Assert.Equal("usd", options.Currency);
    }
}
