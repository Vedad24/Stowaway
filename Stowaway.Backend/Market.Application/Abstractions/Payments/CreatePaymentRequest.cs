namespace Market.Application.Abstractions.Payments
{
    public class CreatePaymentRequest
    {
        public int OrderId { get; init; }

        public decimal Amount { get; init; }

        public string Currency { get; init; } = "bam";

        public string SuccessUrl { get; init; } = string.Empty;

        public string CancelUrl { get; init; } = string.Empty;
    }
}