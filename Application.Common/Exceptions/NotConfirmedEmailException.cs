namespace Application.Common.Exceptions;

public class NotConfirmedEmailException : BaseApplicationException
{
    public NotConfirmedEmailException(string? message)
        : base(message)
    {
    }
}
