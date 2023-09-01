using Application.Services.Models.Categories;

namespace Application.Services.Categories;

public interface ICategoryService
{
    Task CreteCategoryAsync(CreateCategoryRequestModel requestModel);

    Task CreateSubCategoriesAsync(CreateSubCategoryRequestModel requestModel);
}
