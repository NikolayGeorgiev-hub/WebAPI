namespace Application.Common.Exceptions.Products;

public class NotFoundCategoryException : BaseApplicationException
{
    public NotFoundCategoryException(string? message)
        : base(message)
    {
    }
}
