using Application.Common.Exceptions.Orders;
using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Services.Extensions;
using Application.Services.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Orders;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddProductAsync(Guid userId, Guid productId)
    {
        Order? order = await this.dbContext.Orders
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == OrderStatus.InProgress);

        bool existsProduct = await this.dbContext.Products.AnyAsync(x => x.Id == productId && x.InStock == true);
        if (!existsProduct)
        {
            throw new NotFoundProductException("Not found product");
        }

        if (order is null)
        {
            order = new()
            {
                CreatedOn = DateTime.UtcNow,
                Status = OrderStatus.InProgress,
                UserId = userId,
            };

            await this.dbContext.Orders.AddAsync(order);
        }

        ProductsList? productInOrder = order.Products
            .FirstOrDefault(x => x.OrderId == order.Id && x.ProductId == productId);

        if (productInOrder is not null)
        {
            productInOrder.Quantity++;
        }
        else
        {
            productInOrder = new()
            {
                OrderId = order.Id,
                ProductId = productId,
                Quantity = 1
            };

            await this.dbContext.ProductsLists.AddAsync(productInOrder);
        }

        await this.dbContext.SaveChangesAsync();
    }

    public async Task EditProductsQuantityAsync(Guid userId, Guid productId, string action)
    {
        Order? order = await this.dbContext.Orders
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == OrderStatus.InProgress);

        if (order is null)
        {
            throw new NotFoundOrderException("Not found exception");
        }

        bool existsProduct = await this.dbContext.Products.AnyAsync(x => x.Id == productId && x.InStock == true);
        if (!existsProduct)
        {
            throw new NotFoundProductException("Not found product");
        }

        ProductsList? productInOrder = order.Products
            .FirstOrDefault(x => x.OrderId == order.Id && x.ProductId == productId);

        if (productInOrder is null)
        {
            throw new NotFoundProductInOrder("Not found product in current order");
        }

        if (action == "increase-count")
        {
            productInOrder.Quantity++;
            if (productInOrder.Quantity > productInOrder.Product.Quantity)
            {
                throw new ProductOutOfStockException("Product out of stock");
            }
        }

        if (action == "decrease-count")
        {
            productInOrder.Quantity--;
            if (productInOrder.Quantity == 0)
            {
                this.dbContext.ProductsLists.Remove(productInOrder);
            }
        }

        await this.dbContext.SaveChangesAsync();
    }

    public async Task RemoveProductAsync(Guid userId, Guid productId)
    {
        Order? order = await this.dbContext.Orders
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == OrderStatus.InProgress);

        if (order is null)
        {
            throw new NotFoundOrderException("Not found exception");
        }

        bool existsProduct = await this.dbContext.Products.AnyAsync(x => x.Id == productId && x.InStock == true);
        if (!existsProduct)
        {
            throw new NotFoundProductException("Not found product");
        }

        ProductsList? productInOrder = order.Products
            .FirstOrDefault(x => x.OrderId == order.Id && x.ProductId == productId);

        if (productInOrder is null)
        {
            throw new NotFoundProductInOrder("Not found product in current order");
        }

        this.dbContext.ProductsLists.Remove(productInOrder);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<OrderDetailsResponseModel> OrderDetailsAsync(Guid userId)
    {
        Order? order = await this.dbContext.Orders
            .Where(x => x.Products.Count > 0)
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (order is null)
        {
            throw new NotFoundOrderException("Not found exception");
        }

        IReadOnlyList<ProductInOrderResponseModel> productsResponse = order.Products
            .Select(x => x.ToProductInOrder()).ToList();

        OrderDetailsResponseModel orderDetails = order.ToOrderDetails(productsResponse);

        return orderDetails;
    }
}
