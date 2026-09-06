using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options
{
    public class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        public sealed class PolicySettings
        {
            [Range(1, int.MaxValue)]
            public int PermitLimit { get; set; }

            [Range(1, int.MaxValue)]
            public int WindowSeconds { get; set; }

            [Range(0, int.MaxValue)]
            public int QueueLimit { get; set; }
        }

        [Required]
        public PolicySettings Global { get; set; } = null!;

        [Required]
        public PolicySettings Auth { get; set; } = null!;
    }
}
