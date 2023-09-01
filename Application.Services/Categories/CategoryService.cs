using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Categories;
using Application.Services.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext dbContext;

    public CategoryService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreteCategoryAsync(CreateCategoryRequestModel requestModel)
    {
        bool existsCategoryName = await this.dbContext.Categories.AnyAsync(x => x.Name == requestModel.Name);
        if (existsCategoryName)
        {
            throw new ExistsCategoryNameException("Category name already exists");
        }

        Category category = new()
        {
            Name = requestModel.Name,
        };

        await this.dbContext.Categories.AddAsync(category);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task CreateSubCategoriesAsync(CreateSubCategoryRequestModel requestModel)
    {
        bool existsCategoryName = await this.dbContext.SubCategories.AnyAsync(x => x.Name == requestModel.Name);
        if (existsCategoryName)
        {
            throw new ExistsCategoryNameException("Category name already exists");
        }

        bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
        if (!existsCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }

        SubCategory subCategory = new()
        {
            Name = requestModel.Name,
            CategoryId = requestModel.CategoryId,
        };

        await this.dbContext.SubCategories.AddAsync(subCategory);
        await this.dbContext.SaveChangesAsync();
    }
}
