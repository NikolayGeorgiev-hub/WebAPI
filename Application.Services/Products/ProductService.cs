using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Products;
using Application.Services.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Products;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreteProductAsync(Guid ownerId, CreateProductRequestModel requestModel)
    {
        await this.ValidateCreateProductRequestAsync(requestModel);

        Product product = new()
        {
            Name = requestModel.Name,
            Description = requestModel.Description,
            Price = requestModel.Price,
            CategoryId = requestModel.CategoryId,
            SubCategoryId = requestModel.SubCategoryId,
            OwnerId = ownerId,
        };

        await this.dbContext.AddAsync(product);
        await this.dbContext.SaveChangesAsync();
    }



    private async Task ValidateCreateProductRequestAsync(CreateProductRequestModel requestModel)
    {
        bool existsProductName = await this.dbContext.Products.AnyAsync(x => x.Name == requestModel.Name);
        if (existsProductName)
        {
            throw new ExistsProductNameException("Name already exists");
        }

        bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
        if (!existsCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }

        bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId);
        if (!existsSubCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }

        bool isValidSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId && x.CategoryId == requestModel.CategoryId);
        if (!isValidSubCategory)
        {
            throw new NotFoundCategoryException("Not found category");
        }
    }
}
