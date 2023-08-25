namespace Application.Common.Exceptions;

public class NotFoundUserException : BaseApplicationException
{
    public NotFoundUserException(string? message) 
        : base(message)
    {
    }
}
