namespace Application.Common.Exceptions;

public class InvalidModelStateException : BaseApplicationException
{
    public string[] Errors { get; }

    public InvalidModelStateException(string? message, string[] errors) 
        : base(message)
    {
        Errors = errors;
    }
}
