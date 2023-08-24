namespace Application.Common.Exceptions
{
    public class InvalidLoginException : BaseApplicationException
    {
        public InvalidLoginException(string? message)
            : base(message)
        {
        }
    }
}
