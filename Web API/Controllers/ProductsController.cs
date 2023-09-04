using Application.Common;
using Application.Common.Extensions;
using Application.Services.Models;
using Application.Services.Models.Products;
using Application.Services.Models.Ratings;
using Application.Services.Products;
using Application.Services.Ratings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;
    private readonly IRatingService ratingService;

    public ProductsController(IProductService productService, IRatingService ratingService)
    {
        this.productService = productService;
        this.ratingService = ratingService;
    }

    [HttpGet("all")]
    public async Task<ResponseContent<PaginationResponseModel<ProductResponseModel>>> GetAllProductsAsync([FromQuery] ProductsFilter productsFilter)
    {
        var products = await productService.GetAllProductsAsync(productsFilter);
        return new ResponseContent<PaginationResponseModel<ProductResponseModel>>
        {
            Result = products
        };
    }

    [Authorize]
    [HttpPost("rate")]
    public async Task<ResponseContent> RateProductAsync([FromBody] RatingRequestModel requestModel)
    {
        Guid userId = ClaimsPrincipalExtensions.GetUserId(this.User);
        await this.ratingService.RateProductAsync(userId, requestModel);

        return new ResponseContent();
    }
}
