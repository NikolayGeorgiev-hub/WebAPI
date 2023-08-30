namespace Application.Common.Exceptions;

public class ExistsRoleNameException : BaseApplicationException
{
    public ExistsRoleNameException(string? message) 
        : base(message)
    {
    }
}
