using Application.Common.Exceptions.Orders;
using Application.Common.Exceptions.Products;
using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Repositories.Orders;
using Application.Data.Repositories.Products;
using Application.Services.Extensions;
using Application.Services.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;
    private readonly IProductRepository productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        this.orderRepository = orderRepository;
        this.productRepository = productRepository;
    }

    public async Task AddProductAsync(Guid userId, Guid productId)
    {
        Order? order = await this.orderRepository.GetUserOrderInProgressAsync(userId);

        await this.ValidateExistsProductAsync(productId);

        order = await this.AddOrderAsync(order, userId);
        await this.AddOrUpdateProductInOrderAsync(order!, productId);

        await this.orderRepository.SaveChangesAsync();
    }

    public async Task EditProductsQuantityAsync(Guid userId, Guid productId, string action)
    {
        Order? order = await this.GetUserOrderAsync(userId);
        await this.ValidateExistsProductAsync(productId);

        ProductsList productInOrder = this.ValidateProductIsInOrder(order, productId);

        if (action == "increase-count")
        {
            productInOrder.Quantity++;
            int currentProductQuantity = productInOrder.Product.Quantity;

            if (productInOrder.Quantity > currentProductQuantity)
                throw new ProductOutOfStockException("Product out of stock");
        }

        if (action == "decrease-count")
        {
            productInOrder.Quantity--;
            if (productInOrder.Quantity == 0)
                this.orderRepository.RemoveProductsFromOrder(productInOrder);
        }

        await this.orderRepository.SaveChangesAsync();
    }

    public async Task RemoveProductAsync(Guid userId, Guid productId)
    {
        Order order = await GetUserOrderAsync(userId);

        await this.ValidateExistsProductAsync(productId);

        ProductsList productInOrder = this.ValidateProductIsInOrder(order, productId);

        this.orderRepository.RemoveProductsFromOrder(productInOrder);
        await this.orderRepository.SaveChangesAsync();
    }

    public async Task<OrderDetailsResponseModel> OrderDetailsAsync(Guid userId)
    {
        Order order = await GetUserOrderAsync(userId);

        return GetOrderDetails(order);
    }

    public async Task<OrderDetailsResponseModel> SendOrderAsync(Guid userId)
    {
        Order order = await GetUserOrderAsync(userId);

        await EditProductQuantityAsync(order);

        order.CreatedOn = DateTime.UtcNow;
        order.Status = OrderStatus.Send;

        await this.orderRepository.SaveChangesAsync();

        return GetOrderDetails(order);
    }

    public async Task CancelOrderAsync(Guid userId, Guid orderId)
    {
        Order? order = await GetUserOrderAsync(userId);

        if (order is null)
            throw new NotFoundOrderException("Not found order");


        order.Status = OrderStatus.Canceled;

        foreach (var productInOrder in order.Products)
        {
            int newQuantity = productInOrder.Product.Quantity + productInOrder.Quantity;
            productInOrder.Product.Quantity = newQuantity;

            if (!productInOrder.Product.InStock)
            {
                productInOrder.Product.InStock = true;
            }
        }

        await this.orderRepository.SaveChangesAsync();
    }

    private async Task<Order> AddOrderAsync(Order? order, Guid userId)
    {
        if (order is null)
        {
            order = new()
            {
                CreatedOn = DateTime.UtcNow,
                Status = OrderStatus.InProgress,
                UserId = userId,
            };

            await this.orderRepository.AddAsync(order);
        }

        return order;
    }

    private async Task AddOrUpdateProductInOrderAsync(Order order, Guid productId)
    {
        ProductsList? productInOrder = order!.Products
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

            await this.orderRepository.AddProductToOrderAsync(productInOrder);
        }
    }

    private async Task ValidateExistsProductAsync(Guid productId)
    {
        bool existsProduct = await this.productRepository.ExistsProductInStockAsync(productId);
        if (!existsProduct)
            throw new NotFoundProductException("Not found product");
    }

    private ProductsList ValidateProductIsInOrder(Order order, Guid productId)
    {
        ProductsList? productInOrder = order.Products
            .FirstOrDefault(x => x.OrderId == order.Id && x.ProductId == productId);

        if (productInOrder is null)
            throw new NotFoundProductInOrder("Not found product in current order");


        return productInOrder;
    }

    private async Task EditProductQuantityAsync(Order order)
    {
        foreach (var productInOrder in order.Products)
        {
            Product product = productInOrder.Product;
            int newProductQuantity = product.Quantity - productInOrder.Quantity;

            if (newProductQuantity == 0)
            {
                if (productInOrder is not null)
                {
                    this.orderRepository.RemoveProductsFromOrder(productInOrder);
                }

                product.InStock = false;
            }

            product.Quantity = newProductQuantity;
        }
    }

    private async Task<Order> GetUserOrderAsync(Guid userId)
    {
        Order? order = await this.orderRepository.GetUserOrderInProgressAsync(userId);

        if (order is null)
        {
            throw new NotFoundOrderException("Not found order");
        }

        return order;
    }

    private OrderDetailsResponseModel GetOrderDetails(Order order)
    {
        IReadOnlyList<ProductInOrderResponseModel> productsResponse = order.Products
            .Select(productInfo => productInfo.ToProductInOrderInfo())
            .ToList();


        decimal totalPrice = productsResponse.Sum(x => x.TotalPrice);
        decimal newTotalPrice = 0;
        decimal difference = 0;

        if (productsResponse.Any(x => x.TotalPriceDiscount is not null))
        {
            IList<ProductsList> productsWithDiscount = order.Products.Where(x => x.Product.DiscountId is not null).ToList();
            IList<ProductsList> productsWithOutDiscount = order.Products.Where(x => x.Product.DiscountId is null).ToList();

            decimal productsWithOutDiscountTotalPrice = productsWithOutDiscount
                .Select(productInfo => new
                {
                    productInfo.Quantity,
                    productInfo.Product
                })
                .Sum(calculation => calculation.Quantity * calculation.Product.Price);


            decimal productsWithDiscountTotalPrice = productsWithDiscount
                .Select(productInfo => new
                {
                    productInfo.Quantity,
                    productInfo.Product
                })
                .Sum(calculation => calculation.Quantity * calculation.Product.NewPrice!.Value);


            newTotalPrice = productsWithOutDiscountTotalPrice + productsWithDiscountTotalPrice;
            difference = totalPrice - newTotalPrice;
        }

        OrderDetailsResponseModel orderDetails = order.ToOrderDetails(
            totalPrice,
            newTotalPrice,
            difference,
            productsResponse);

        return orderDetails;
    }
}
