namespace Market.Application.Common.Exceptions;

public sealed class StowawayUnauthorizedException : Exception
{
    public StowawayUnauthorizedException(string message) : base(message) { }
}
