using Application.Common.Exceptions.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task ExistsCategoryByIdAsync(Guid categoryId)
    {
        bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == categoryId);
        if (!existsCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }
    }

    public async Task ExistsSubCategoryByIdAsync(Guid subCategoryId)
    {
        bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == subCategoryId);
        if (!existsSubCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }
    }

    public async Task CategoryContainsSubCategoryAsync(Guid subCategoryId,Guid categoryId)
    {
        bool isValidSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == subCategoryId && x.CategoryId == categoryId);
        if (!isValidSubCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }
    }
}
