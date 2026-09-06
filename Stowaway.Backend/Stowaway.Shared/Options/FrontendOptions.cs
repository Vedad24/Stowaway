using System.ComponentModel.DataAnnotations;

namespace Stowaway.Shared.Options;

/// <summary>Typed frontend settings from the "Frontend" section.</summary>
public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required] public string BaseUrl { get; init; } = default!;
}
