namespace Application.Common.Exceptions;

public class NotFoundRoleException : BaseApplicationException
{
    public NotFoundRoleException(string? message) : base(message)
    {
    }
}
