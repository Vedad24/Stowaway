namespace Stowaway.Application.Common.Exceptions;

public sealed class StowawayConflictException : Exception
{
    public StowawayConflictException(string message) : base(message) { }
}
