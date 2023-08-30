namespace Application.Common.Exceptions;

public class UserInRoleException : BaseApplicationException
{
    public UserInRoleException(string? message) 
        : base(message)
    {
    }
}
