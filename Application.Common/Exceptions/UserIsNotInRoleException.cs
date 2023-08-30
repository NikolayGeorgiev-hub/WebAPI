namespace Application.Common.Exceptions;

public class UserIsNotInRoleException : BaseApplicationException
{
    public UserIsNotInRoleException(string? message)
        : base(message)
    {
    }
}
