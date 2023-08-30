using Application.Common;
using Application.Services.Administration;
using Application.Services.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]/roles")]
[ApiController]
public class AdministrationController : ControllerBase
{
    private readonly IRoleService roleService;

    public AdministrationController(IRoleService roleService)
    {
        this.roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<ResponseContent> CreateRoleAsync(RoleRequestModels.Crete requestModel)
    {
        await this.roleService.CreateRoleAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPut("edit")]
    public async Task<ResponseContent> EditRoleAsync([FromQuery] Guid roleId, [FromBody] RoleRequestModels.Edit requestModel)
    {
        await this.roleService.EditRoleAsync(roleId, requestModel);
        return new ResponseContent();
    }

    [HttpDelete("remove")]
    public async Task<ResponseContent> RemoveRoleAsync([FromQuery] Guid roleId)
    {
        await roleService.RemoveRoleAsync(roleId);
        return new ResponseContent();
    }

    [HttpPost("assign-users")]
    public async Task<ResponseContent> AssignUsersToRoleAsync([FromBody] RoleRequestModels.AssignUsers requestModel)
    {
        await this.roleService.AssignUsersToRoleAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPost("remove-users")]
    public async Task<ResponseContent> RemoveUsersFromRoleAsync([FromBody] RoleRequestModels.RemoveUsers requestModel)
    {
        await this.roleService.RemoveUsersFromRoleAsync(requestModel);
        return new ResponseContent();
    }


}
