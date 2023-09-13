namespace Application.Common.Exceptions.Discounts;

public class NotFoundDiscountException : BaseApplicationException
{
    public NotFoundDiscountException(string? message) 
        : base(message)
    {
    }
}
