namespace Application.Data.Models.Comments;

public class Comment
{
    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Content { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }
}
