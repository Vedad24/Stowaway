namespace Market.Application.Abstractions.Payments
{
    public class CreatePaymentResult
    {
        public string ExternalPaymentId { get; init; } = string.Empty;

        public string CheckoutUrl { get; init; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}