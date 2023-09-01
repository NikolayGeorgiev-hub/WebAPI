namespace Application.Common.Exceptions.Products;

public class ExistsCategoryNameException : BaseApplicationException
{
    public ExistsCategoryNameException(string? message)
        : base(message)
    {
    }
}
