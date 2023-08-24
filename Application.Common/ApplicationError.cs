namespace Application.Common;

public class ApplicationError
{
    public ApplicationError(string message, int errorCode)
    {
        Message = message;
        ErrorCode = errorCode;
    }

    public string Message { get; set; }

    public int ErrorCode { get; set; }
}
