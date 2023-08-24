namespace Application.Common.Exceptions;

public class ExistsEmailAddressException : BaseApplicationException
{
    public ExistsEmailAddressException(string? message) 
        : base(message)
    {
    }
}
