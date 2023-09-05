namespace Application.Common.Exceptions.Orders;

public class NotFoundOrderException : BaseApplicationException
{
    public NotFoundOrderException(string? message)
        : base(message)
    {
    }
}
