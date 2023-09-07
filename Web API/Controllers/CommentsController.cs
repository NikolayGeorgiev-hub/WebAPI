using Application.Common;
using Application.Common.Extensions;
using Application.Services.Comments;
using Application.Services.Models;
using Application.Services.Models.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService commentService;

    public CommentsController(ICommentService commentService)
    {
        this.commentService = commentService;
    }

    [HttpPost("add/{productId}")]
    public async Task<ResponseContent> AddCommentAsync([FromRoute] Guid productId, AddCommentRequestModel requestModel)
    {
        await this.commentService.AddCommentAsync(GetUserId(), productId, requestModel);
        return new ResponseContent();
    }

    [HttpGet("{productId}")]
    public async Task<ResponseContent<PaginationResponseModel<CommentResponseModel>>> GetAllAsync([FromQuery] PaginationRequestModel requestModel, [FromRoute] Guid productId)
    {
        PaginationResponseModel<CommentResponseModel> result = await this.commentService.GetAllCommentsAsync(productId, requestModel);
        return new ResponseContent<PaginationResponseModel<CommentResponseModel>>()
        {
            Result = result
        };
    }

    private Guid GetUserId()
       => ClaimsPrincipalExtensions.GetUserId(this.User);
}
