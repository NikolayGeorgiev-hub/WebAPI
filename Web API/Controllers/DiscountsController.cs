using Application.Common;
using Application.Services.Discounts;
using Application.Services.Models.Discounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService discountService;

    public DiscountsController(IDiscountService discountService)
    {
        this.discountService = discountService;
    }

    [HttpPost("create")]
    public async Task<ResponseContent> CreateDiscountAsync([FromBody] CreteDiscountRequestModel requestModel)
    {
        await this.discountService.CreateDiscountAsync(requestModel);
        return new ResponseContent();
    }

    [HttpDelete("remove/{discountId}")]
    public async Task<ResponseContent> RemoveDiscountAsync([FromRoute] Guid discountId)
    {
        await this.discountService.RemoveDiscountAsync(discountId);
        return new ResponseContent();
    }
}
