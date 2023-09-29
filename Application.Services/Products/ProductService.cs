using Application.Common.Exceptions.Products;
using Application.Common.Models;
using Application.Data;
using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Models.Ratings;
using Application.Data.Repositories.Categories;
using Application.Data.Repositories.Orders;
using Application.Data.Repositories.Products;
using Application.Services.Comments;
using Application.Services.Extensions;
using Application.Services.Models.Comments;
using Application.Services.Models.Products;
using Application.Services.Models.Ratings;
using Application.Services.Ratings;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IOrderRepository orderRepository;
    private readonly IRatingService ratingService;
    private readonly ICommentService commentService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IOrderRepository orderRepository,
        IRatingService ratingService,
        ICommentService commentService)
    {
        this.productRepository = productRepository;
        this.categoryRepository = categoryRepository;
        this.orderRepository = orderRepository;
        this.ratingService = ratingService;
        this.commentService = commentService;
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

        await this.productRepository.AddAsync(product);
        await this.productRepository.SaveChangesAsync();
    }

    public async Task<PaginationResponseModel<ProductResponseModel>> GetAllProductsAsync(ProductsFilter productsFilter)
    {
        IReadOnlyList<Product> products = await this.productRepository.GetAllAsync(productsFilter);

        IReadOnlyList<ProductResponseModel> productsResponse = products.Select(product
            => product.ToProductResponseModel(CalculateProductRatings(product.Ratings))).ToList();

        int productsCount = await this.productRepository.GetCountAsync(productsFilter);
        int pagesCount = (int)Math.Ceiling((double)productsCount / productsFilter.ItemsPerPage!.Value);

        return new PaginationResponseModel<ProductResponseModel>
        {
            Items = productsResponse,
            TotalItems = productsCount,
            PageNumber = productsFilter.PageNumber!.Value,
            ItemsPerPage = productsFilter.ItemsPerPage!.Value,
            PagesCount = pagesCount,
        };

    }

    public async Task<ProductDetailsResponseModel> GetProductDetailsAsync(Guid productId, PaginationRequestModel requestModel)
    {
        Product? product = await this.productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            throw new NotFoundProductException("Not found product");
        }

        PaginationResponseModel<CommentResponseModel> comments = await this.commentService.GetAllCommentsAsync(productId, requestModel);

        ProductDetailsResponseModel productDetails = product.ToProductDetailsResponseModel(CalculateProductRatings(product.Ratings), comments);


        return productDetails;
    }

    public async Task EditProductAsync(Guid ownerId, Guid productId, EditProductRequestModel requestModel)
    {
        Product? product = await this.productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            throw new NotFoundProductException("Not found product");
        }

        await ValidateEditProductRequestAsync(requestModel, product);

        product.Name = requestModel.Name;
        product.Description = requestModel.Description;
        product.Price = requestModel.Price;
        product.Quantity = requestModel.Quantity;
        product.CategoryId = requestModel.CategoryId;
        product.SubCategoryId = requestModel.SubCategoryId;
        product.InStock = requestModel.InStock;

        if (requestModel.Quantity == 0)
        {
            product.InStock = false;
        }

        await this.orderRepository.RemoveProductWhenOutOfStockAsync(productId);
        await this.productRepository.SaveChangesAsync();
    }

    private RatingResponseModel CalculateProductRatings(ICollection<Rating> ratings)
    {
        int productRatingsCount = ratings.Count > 0 ? ratings.Count : 0;
        double averageProductRating = ratings.Count > 0 ? ratings.Select(x => x.Value).Average() : 0;

        RatingResponseModel productRating = new(productRatingsCount, averageProductRating);

        return productRating;
    }

    private async Task ValidateCreateProductRequestAsync(CreateProductRequestModel requestModel)
    {
        await this.productRepository.ExistsProductNameAsync(requestModel.Name);
        await this.categoryRepository.ExistsCategoryByIdAsync(requestModel.CategoryId);
        await this.categoryRepository.ExistsSubCategoryByIdAsync(requestModel.SubCategoryId);
        await this.categoryRepository.CategoryContainsSubCategoryAsync(requestModel.SubCategoryId, requestModel.CategoryId);
    }

    private async Task ValidateEditProductRequestAsync(EditProductRequestModel requestModel, Product product)
    {
        if (product.Name != requestModel.Name)
        {
            await this.productRepository.ExistsProductNameWhenUpdateAsync(requestModel.Name, product.Id);
        }

        await this.categoryRepository.ExistsCategoryByIdAsync(requestModel.CategoryId);
        await this.categoryRepository.ExistsSubCategoryByIdAsync(requestModel.SubCategoryId);
        await this.categoryRepository.CategoryContainsSubCategoryAsync(requestModel.SubCategoryId, requestModel.CategoryId);
    }
}
