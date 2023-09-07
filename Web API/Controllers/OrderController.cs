using Application.Common;
using Application.Common.Extensions;
using Application.Data.Models.Products;
using Application.Services.Models.Orders;
using Application.Services.Orders;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web_API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }


    [HttpPost("add-product/{productId}")]
    public async Task<ResponseContent> AddProductAsync([FromRoute] Guid productId)
    {
        await this.orderService.AddProductAsync(GetUserId(), productId);
        return new ResponseContent();
    }


    [HttpPut("increase-count/{productId}")]
    public async Task<ResponseContent> IncreaseProductCountAsync([FromRoute] Guid productId)
    {
        await this.orderService.EditProductsQuantityAsync(GetUserId(), productId, "increase-count");
        return new ResponseContent();
    }


    [HttpPut("decrease-count/{productId}")]
    public async Task<ResponseContent> DecreaseProductCountAsync([FromRoute] Guid productId)
    {
        await this.orderService.EditProductsQuantityAsync(GetUserId(), productId, "decrease-count");

        return new ResponseContent();
    }

    [HttpDelete("remove-product/{productId}")]
    public async Task<ResponseContent> RemoveProductAsync([FromRoute] Guid productId)
    {
        await orderService.RemoveProductAsync(GetUserId(), productId);
        return new ResponseContent();
    }

    [HttpGet("details")]
    public async Task<ResponseContent<OrderDetailsResponseModel>> OrderDetailsAsync()
    {
        OrderDetailsResponseModel orderDetails = await this.orderService.OrderDetailsAsync(GetUserId());
        return new ResponseContent<OrderDetailsResponseModel>()
        {
            Result = orderDetails
        };
    }

    [HttpPost("send")]
    public async Task<ResponseContent<OrderDetailsResponseModel>> SendOrderAsync()
    {
        OrderDetailsResponseModel orderDetails = await this.orderService.SendOrderAsync(GetUserId());
        return new ResponseContent<OrderDetailsResponseModel>()
        {
            Result = orderDetails
        };
    }

    private Guid GetUserId()
        => ClaimsPrincipalExtensions.GetUserId(this.User);
}
