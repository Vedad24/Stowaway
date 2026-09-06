using Market.Shared.Dtos;
using Market.Shared.Options;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Market.API;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var rateLimiting = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()!;
        var global = rateLimiting.Global;
        var auth = rateLimiting.Auth;

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // General overload protection: partitioned per user (once authenticated) or per IP.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var key = httpContext.User.Identity?.IsAuthenticated == true
                    ? $"u:{httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"
                    : $"ip:{httpContext.Connection.RemoteIpAddress}";

                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = global.PermitLimit,
                    Window = TimeSpan.FromSeconds(global.WindowSeconds),
                    QueueLimit = global.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Brute-force protection for login/refresh, keyed by IP since these run pre-authentication.
            options.AddPolicy("auth", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter($"ip:{httpContext.Connection.RemoteIpAddress}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = auth.PermitLimit,
                    Window = TimeSpan.FromSeconds(auth.WindowSeconds),
                    QueueLimit = auth.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));

            options.OnRejected = async (context, ct) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                }

                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new ErrorDto
                {
                    Code = "rate_limit.exceeded",
                    Message = "Too many requests. Please try again later."
                }, ct);
            };
        });

        return services;
    }
}
