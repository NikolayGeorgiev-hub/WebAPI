using System.Net;

namespace Application.Common.Exceptions;

public class BaseApplicationException : Exception
{
    public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
    public BaseApplicationException(string? message)
        : base(message)
    {
    }
}
