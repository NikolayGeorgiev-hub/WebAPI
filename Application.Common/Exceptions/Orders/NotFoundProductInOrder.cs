namespace Application.Common.Exceptions.Orders;

public class NotFoundProductInOrder : BaseApplicationException
{
    public NotFoundProductInOrder(string? message)
        : base(message)
    {
    }
}
