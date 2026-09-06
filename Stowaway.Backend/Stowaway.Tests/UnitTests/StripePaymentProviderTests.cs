using Stowaway.Infrastructure.Payments.Stripe;

namespace Stowaway.Tests.UnitTests;

public class StripePaymentProviderTests
{
    [Fact]
    public void BuildCheckoutIdempotencyKey_IsDeterministic_ForSameOrderId()
    {
        var first = StripePaymentProvider.BuildCheckoutIdempotencyKey(42);
        var second = StripePaymentProvider.BuildCheckoutIdempotencyKey(42);

        Assert.Equal(first, second);
    }

    [Fact]
    public void BuildCheckoutIdempotencyKey_Differs_AcrossOrderIds()
    {
        var key1 = StripePaymentProvider.BuildCheckoutIdempotencyKey(1);
        var key2 = StripePaymentProvider.BuildCheckoutIdempotencyKey(2);

        Assert.NotEqual(key1, key2);
    }
}
