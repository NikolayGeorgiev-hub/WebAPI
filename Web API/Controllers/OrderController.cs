using Application.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }
}
