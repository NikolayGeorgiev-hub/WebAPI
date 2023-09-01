using Application.Common;
using Application.Services.Categories;
using Application.Services.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        this.categoryService = categoryService;
    }

    [HttpPost("create")]
    public async Task<ResponseContent> CreteCategoryAsync([FromBody] CreateCategoryRequestModel requestModel)
    {
        await this.categoryService.CreteCategoryAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPost("sub/create")]
    public async Task<ResponseContent> CreateSubCategoryAsync([FromBody] CreateSubCategoryRequestModel requestModel)
    {
        await this.categoryService.CreateSubCategoriesAsync(requestModel);
        return new ResponseContent();
    }
}
