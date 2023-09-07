using Application.Services.Models;
using Application.Services.Models.Comments;

namespace Application.Services.Comments;

public interface ICommentService
{
    Task AddCommentAsync(Guid userId, Guid productId, AddCommentRequestModel requestModel);

    Task<PaginationResponseModel<CommentResponseModel>> GetAllCommentsAsync(Guid productId, PaginationRequestModel requestModel);
}
