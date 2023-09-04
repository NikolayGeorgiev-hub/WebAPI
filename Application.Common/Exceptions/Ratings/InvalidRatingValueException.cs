namespace Application.Common.Exceptions.Ratings;

public class InvalidRatingValueException : BaseApplicationException
{
    public InvalidRatingValueException(string? message) 
        : base(message)
    {
    }
}
