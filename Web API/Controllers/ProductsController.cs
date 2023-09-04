using Application.Common;
using Application.Services.Models;
using Application.Services.Models.Products;
using Application.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;

    public ProductsController(IProductService productService)
    {
        this.productService = productService;
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
}
