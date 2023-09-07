namespace Application.Common.Exceptions.Comments;

public class NotFoundCommentException : BaseApplicationException
{
    public NotFoundCommentException(string? message) 
        : base(message)
    {
    }
}
