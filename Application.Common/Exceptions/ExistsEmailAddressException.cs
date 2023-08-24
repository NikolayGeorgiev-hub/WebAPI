namespace Application.Common.Exceptions;

public class ExistsEmailAddressException : Exception
{
    public ExistsEmailAddressException(string? message) 
        : base(message)
    {
    }
}
