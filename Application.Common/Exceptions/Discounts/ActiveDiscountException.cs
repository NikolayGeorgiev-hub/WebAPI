namespace Application.Common.Exceptions.Discounts;

public class ActiveDiscountException : BaseApplicationException
{
    public ActiveDiscountException(string? message) 
        : base(message)
    {
    }
}
