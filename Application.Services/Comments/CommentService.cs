using Application.Common.Exceptions.Comments;
using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Comments;
using Application.Services.Models;
using Application.Services.Models.Comments;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Comments;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext dbContext;

    public CommentService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddCommentAsync(Guid userId, Guid productId, AddCommentRequestModel requestModel)
    {
        bool existsProduct = await this.dbContext.Products.AnyAsync(x => x.Id == productId);
        if (!existsProduct)
        {
            throw new NotFoundProductException("Not found product");
        }

        Comment comment = new Comment()
        {
            CreatedOn = DateTime.UtcNow,
            UserId = userId,
            ProductId = productId,
            Content = requestModel.Content
        };

        await dbContext.Comments.AddAsync(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PaginationResponseModel<CommentResponseModel>> GetAllCommentsAsync(Guid productId, PaginationRequestModel requestModel)
    {
        IQueryable<Comment> commentsQuery = this.dbContext.Comments
            .Where(x => x.ProductId == productId)
            .Include(x => x.User);

        int totalCount = await commentsQuery.CountAsync();

        commentsQuery = commentsQuery
            .Skip(requestModel.SkipCount)
            .Take(requestModel.ItemsPerPage!.Value);

        int pagesCount = (int)Math.Ceiling((double)totalCount / requestModel.ItemsPerPage!.Value);

        IReadOnlyList<CommentResponseModel> commentResponses = await commentsQuery
            .Select(comment => new CommentResponseModel(
                comment.CreatedOn,
                comment.User.FirstName!,
                comment.Content))
            .ToListAsync();

        return new PaginationResponseModel<CommentResponseModel>
        {
            Items = commentResponses,
            TotalItems = totalCount,
            PageNumber = requestModel.PageNumber!.Value,
            ItemsPerPage = requestModel.ItemsPerPage!.Value,
            PagesCount = pagesCount,
        };
    }

    public async Task DeleteCommentAsync(Guid userId, Guid commentId)
    {
        Comment? comment = await this.dbContext.Comments.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == commentId);
        if (comment is null)
        {
            throw new NotFoundCommentException("Not found comment");
        }

        this.dbContext.Comments.Remove(comment);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Guid userId,Guid commentId,AddCommentRequestModel requestModel)
    {
        Comment? comment = await this.dbContext.Comments.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == commentId);
        if (comment is null)
        {
            throw new NotFoundCommentException("Not found comment");
        }

        comment.Content = requestModel.Content;
        await this.dbContext.SaveChangesAsync();
    }
}
