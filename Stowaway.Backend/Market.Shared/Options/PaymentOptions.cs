using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options;

/// <summary>Typed frontend payment-result route settings from the "Payment" section.</summary>
public sealed class PaymentOptions
{
    public const string SectionName = "Payment";

    // Frontend route segment for a successful payment; the order id is always appended after it.
    [Required] public string SuccessPath { get; init; } = default!;

    // Frontend route segment for a cancelled payment; the order id is always appended after it.
    [Required] public string CancelPath { get; init; } = default!;
}
