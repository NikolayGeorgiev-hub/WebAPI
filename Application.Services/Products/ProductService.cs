using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Products;
using Application.Services.Extensions;
using Application.Services.Models;
using Application.Services.Models.Products;
using Application.Services.Ratings;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Products;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IRatingService ratingService;

    public ProductService(
        ApplicationDbContext dbContext,
        IRatingService ratingService)
    {
        this.dbContext = dbContext;
        this.ratingService = ratingService;
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
            .Include(x => x.SubCategory)
            .OrderBy(x => x.Name);

        productsQuery = this.ApplyProductsFilter(productsQuery, productsFilter);

        int totalCount = await productsQuery.CountAsync();

        productsQuery = productsQuery
            .Skip(productsFilter.SkipCount)
            .Take(productsFilter.ItemsPerPage!.Value);

        int pagesCount = (int)Math.Ceiling((double)totalCount / productsFilter.ItemsPerPage!.Value);


        IReadOnlyList<ProductResponseModel> products = await productsQuery
            .Select(product => product.ToProductResponseModel(this.ratingService.GetProductRating(product.Id)))
            .ToListAsync();


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

        if (productsFilter.SubCategories is not null)
        {
            HashSet<IQueryable<Product>> filterResults = new();

            foreach (var subCategoryId in productsFilter.SubCategories)
            {
                IQueryable<Product> tempProductQuery = productsQuery.Where(x => x.SubCategoryId == subCategoryId);
                filterResults.Add(tempProductQuery);
            }

            productsQuery = filterResults.Aggregate((q1, q2) => q1.Union(q2));
        }

        if (productsFilter.SortingFilter is not null)
        {
            switch (productsFilter.SortingFilter)
            {
                case SortingFilter.NameDescending:
                    productsQuery = productsQuery.OrderByDescending(x => x.Name);
                    break;
                case SortingFilter.NameAscending:
                    productsQuery = productsQuery.OrderBy(x => x.Name);
                    break;
                case SortingFilter.PriceDescending:
                    productsQuery = productsQuery.OrderByDescending(x => x.Price);
                    break;
                case SortingFilter.PriceAscending:
                    productsQuery = productsQuery.OrderBy(x => x.Price);
                    break;
                case SortingFilter.RatingDescending:
                    productsQuery = productsQuery.OrderByDescending(x => x.Ratings.Select(x => x.Value).Average());
                    break;
                case SortingFilter.RatingAscending:
                    productsQuery = productsQuery.OrderBy(x => x.Ratings.Select(x => x.Value).Average());
                    break;
                default:
                    break;
            }
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
