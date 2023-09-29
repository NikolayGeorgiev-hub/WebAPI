using Application.Common;
using Application.Common.Extensions;
using Application.Common.Models;
using Application.Services.Comments;
using Application.Services.Models;
using Application.Services.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Authorize]
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
    public async Task<ResponseContent> AddCommentAsync([FromRoute] Guid productId, [FromBody] AddCommentRequestModel requestModel)
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

    [HttpDelete("delete/{commentId}")]
    public async Task<ResponseContent> DeleteAsync([FromRoute] Guid commentId)
    {
        await this.commentService.DeleteCommentAsync(GetUserId(), commentId);
        return new ResponseContent();
    }

    [HttpPut("update/{commentId}")]
    public async Task<ResponseContent> UpdateAsync([FromRoute] Guid commentId, [FromBody] AddCommentRequestModel requestModel)
    {
        await this.commentService.UpdateCommentAsync(GetUserId(), commentId, requestModel);
        return new ResponseContent();
    }

    private Guid GetUserId()
       => ClaimsPrincipalExtensions.GetUserId(this.User);
}
