namespace Market.Application.Common.Exceptions;

public sealed class StowawayNotFoundException : Exception
{
    public StowawayNotFoundException(string message) : base(message) { }
}
