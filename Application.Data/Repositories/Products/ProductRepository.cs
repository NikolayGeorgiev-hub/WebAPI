using Application.Common;
using Application.Common.Exceptions.Products;
using Application.Common.Models;
using Application.Data.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repositories.Products;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(ProductsFilter productsFilter)
    {
        IQueryable<Product> productsQuery = this.dbContext.Products
            .Include(x => x.Category)
            .ThenInclude(x => x.SubCategories)
            .Include(x => x.Ratings);

        productsQuery = this.ApplyProductsFilter(productsQuery, productsFilter);

        int totalCount = await productsQuery.CountAsync();

        productsQuery = productsQuery
            .Skip(productsFilter.SkipCount)
            .Take(productsFilter.ItemsPerPage!.Value);



        return await productsQuery.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        Product? product = await this.dbContext.Products
            .Include(x => x.Category)
            .ThenInclude(x => x.SubCategories)
            .Include(x => x.Ratings)
            .FirstOrDefaultAsync(x => x.Id == id);

        return product;
    }

    public async Task<int> GetCountAsync(ProductsFilter productsFilter)
    {
        IQueryable<Product> productsQuery = this.dbContext.Products
            .Include(x => x.Category)
            .ThenInclude(x => x.SubCategories);

        productsQuery = this.ApplyProductsFilter(productsQuery, productsFilter);

        return await productsQuery.CountAsync();
    }

    public async Task ExistsProductNameAsync(string productName)
    {
        bool existsProductName = await this.dbContext.Products.AnyAsync(x => x.Name == productName);
        if (existsProductName)
        {
            throw new ExistsProductNameException("Name already exists");
        }
    }

    public async Task ExistsProductNameWhenUpdateAsync(string productName,Guid productId)
    {
        bool existsProductName = await this.dbContext.Products.AnyAsync(x => x.Name == productName && x.Id != productId);
        if (existsProductName)
        {
            throw new ExistsProductNameException("Name already exists");
        }
    }

    public async Task SaveChangesAsync()
    {
        await this.dbContext.SaveChangesAsync();
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
}
