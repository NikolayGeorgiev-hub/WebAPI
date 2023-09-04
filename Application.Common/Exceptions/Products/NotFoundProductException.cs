namespace Application.Common.Exceptions.Products;

public class NotFoundProductException : BaseApplicationException
{
    public NotFoundProductException(string? message)
        : base(message)
    {
    }
}
