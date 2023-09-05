namespace Application.Common.Exceptions.Products;

public class ProductOutOfStockException : BaseApplicationException
{
    public ProductOutOfStockException(string? message) : base(message)
    {
    }
}
