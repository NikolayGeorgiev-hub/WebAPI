using Application.Services.Accounts;
using Application.Services.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpPost("registration")]
    public async Task RegistrationAsync([FromBody] UserRegistrationRequestModel requestModel)
    {
        await this.accountService.RegistrationAsync(requestModel);
    }

    [HttpPost("login")]
    public async Task<string> LoginAsync([FromBody] UserLoginRequestModel requestModel)
    {
        string token = await accountService.LoginAsync(requestModel);
        return token;
    }
}
