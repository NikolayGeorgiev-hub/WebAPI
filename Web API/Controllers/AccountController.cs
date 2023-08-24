﻿using Application.Common;
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
    public async Task<ResponseContent> RegistrationAsync([FromBody] UserRegistrationRequestModel requestModel)
    {
        await this.accountService.RegistrationAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPost("login")]
    public async Task<ResponseContent<string>> LoginAsync([FromBody] UserLoginRequestModel requestModel)
    {
        string token = await accountService.LoginAsync(requestModel);
        return new ResponseContent<string>()
        {
            Result = token
        };
    }
}
