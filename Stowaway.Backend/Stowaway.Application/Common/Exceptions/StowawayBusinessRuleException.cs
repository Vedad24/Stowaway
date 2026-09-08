namespace Stowaway.Application.Common.Exceptions;

/// <summary>
/// Represents an error that occurs when a business rule is violated.
///
/// Such errors do not indicate system issues (e.g., null reference),
/// but situations where a request cannot be executed because it would
/// violate business logic.
///
/// </summary>
public sealed class StowawayBusinessRuleException : Exception
{
    public string Code { get; }

    public StowawayBusinessRuleException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public StowawayBusinessRuleException(string code, string message, Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}