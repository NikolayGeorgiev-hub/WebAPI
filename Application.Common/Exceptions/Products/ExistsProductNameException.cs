namespace Application.Common.Exceptions.Products;

public class ExistsProductNameException : BaseApplicationException
{
    public ExistsProductNameException(string? message) 
        : base(message)
    {
    }
}
