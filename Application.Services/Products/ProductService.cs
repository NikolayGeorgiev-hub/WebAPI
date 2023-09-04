using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Products;
using Application.Services.Models;
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
            Quantity = requestModel.Quantity,
            InStock = true,
            CategoryId = requestModel.CategoryId,
            SubCategoryId = requestModel.SubCategoryId,
            OwnerId = ownerId,
        };

        await this.dbContext.AddAsync(product);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<PaginationResponseModel<ProductResponseModel>> GetAllProductsAsync(ProductsFilter productsFilter)
    {
        IQueryable<Product> productsQuery = this.dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.SubCategory);


        productsQuery = this.ApplyProductsFilter(productsQuery, productsFilter);

        int totalCount = await productsQuery.CountAsync();

        productsQuery = productsQuery
            .Skip(productsFilter.SkipCount)
            .Take(productsFilter.ItemsPerPage!.Value);

        int pagesCount = (int)Math.Ceiling((double)totalCount / productsFilter.ItemsPerPage!.Value);

        IReadOnlyList<ProductResponseModel> products = await productsQuery.Select(product => new ProductResponseModel(
            product.Name,
            product.Description,
            product.Price,
            product.Quantity,
            product.Category.Name,
            product.SubCategory.Name, product.InStock)).ToListAsync();

        return new PaginationResponseModel<ProductResponseModel>
        {
            Items = products,
            TotalItems = totalCount,
            PageNumber = productsFilter.PageNumber!.Value,
            ItemsPerPage = productsFilter.ItemsPerPage!.Value,
            PagesCount = pagesCount,
        };

    }

    private IQueryable<Product> ApplyProductsFilter(IQueryable<Product> productsQuery, ProductsFilter productsFilter)
    {
        if (productsFilter.CategoryId is not null)
        {
            productsQuery = productsQuery.Where(x => x.CategoryId == productsFilter.CategoryId);
        }

        if (productsFilter.SearchTerm is not null)
        {
            productsQuery = productsQuery.Where(x => x.Name.Contains(productsFilter.SearchTerm));
        }

        return productsQuery;
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
