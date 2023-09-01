using Application.Common;
using Application.Common.Extensions;
using Application.Services.Models.Products;
using Application.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OwnerController : ControllerBase
{
    private readonly IProductService productService;

    public OwnerController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpPost("create-product")]
    public async Task<ResponseContent> CreteProductAsync(CreateProductRequestModel requestModel)
    {
        await this.productService.CreteProductAsync(ClaimsPrincipalExtensions.GetUserId(this.User), requestModel);
        return new ResponseContent();
    }
}
