using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Data;
using Application.Data.Models.Users;
using Application.Services.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Administration;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<RoleService> logger;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        ILogger<RoleService> logger)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task CreateRoleAsync(RoleRequestModels.Crete requestModel)
    {
        bool existsRoleName = await this.roleManager.RoleExistsAsync(requestModel.Name);
        if (existsRoleName)
        {
            this.logger.LogError("Role name already taken {0}", requestModel.Name);
            throw new ExistsRoleNameException("Role name already exist");
        }

        ApplicationRole role = new()
        {
            Name = requestModel.Name,
        };

        IdentityResult result = await this.roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            this.logger.LogError(IdentityResultExtensions.GetIdentityResultMessages(result));
            //throw new Exception
        }
    }

    public async Task EditRoleAsync(Guid roleId, RoleRequestModels.Edit requestModel)
    {
        ApplicationRole role = await this.GetRoleAsync(roleId);

        bool existsRoleName = await this.dbContext.Roles.AnyAsync(x => x.Name == requestModel.Name && x.Id != roleId);
        if (existsRoleName)
        {
            this.logger.LogError("Role name already taken {0}", requestModel.Name);
            throw new ExistsRoleNameException("Role name already exist");
        }


        role.Name = requestModel.Name;
        await this.roleManager.UpdateAsync(role);
    }

    public async Task RemoveRoleAsync(Guid roleId)
    {
        ApplicationRole role = await this.GetRoleAsync(roleId);

        bool existsUserInRole = await this.dbContext.UserRoles.AnyAsync(x => x.RoleId == roleId);
        if (existsUserInRole)
        {
            this.logger.LogError("Before remove role with id {0} clear role relations", roleId);
            throw new NotFoundRoleException("Does remove this role");
        }

        await this.roleManager.DeleteAsync(role);
    }

    public async Task AssignUsersToRoleAsync(RoleRequestModels.AssignUsers requestModel)
    {
        ApplicationRole role = await this.GetRoleAsync(requestModel.RoleId);
        IList<ApplicationUser> tempUsersList = new List<ApplicationUser>();

        foreach (var userId in requestModel.UsersId)
        {
            ApplicationUser? user = await this.dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                this.logger.LogError("Not found user with id {0}", userId);
                throw new NotFoundUserException("Not found user");
            }

            bool userIsInRole = await this.userManager.IsInRoleAsync(user, role.Name!);
            if (userIsInRole)
            {
                this.logger.LogError("User with id {0} is in role with name {1}", userId, role.Name);
                throw new UserInRoleException("The current user is in role");
            }

            tempUsersList.Add(user);
        }

        foreach (var user in tempUsersList)
        {
            await this.userManager.AddToRoleAsync(user, role.Name!);
        }
    }

    public async Task RemoveUsersFromRoleAsync(RoleRequestModels.RemoveUsers requestModel)
    {
        ApplicationRole role = await this.GetRoleAsync(requestModel.RoleId);

        IList<ApplicationUser> tempUsersList = new List<ApplicationUser>();

        foreach (var userId in requestModel.UsersId)
        {
            ApplicationUser? user = await this.dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                this.logger.LogError("Not found user with id {0}", userId);
                throw new NotFoundUserException("Not found user");
            }

            bool userIsInRole = await this.userManager.IsInRoleAsync(user, role.Name!);
            if (!userIsInRole)
            {
                this.logger.LogError("User with id {0} is in role with name {1}", userId, role.Name);
                throw new UserIsNotInRoleException("The current user is not in the role");
            }

            tempUsersList.Add(user);
        }

        foreach (var user in tempUsersList)
        {
            var rrr = await this.userManager.RemoveFromRoleAsync(user, role.Name!);
        }



    }

    private async Task<ApplicationRole> GetRoleAsync(Guid roleId)
    {
        ApplicationRole? role = await this.roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
        {
            this.logger.LogError("Role with id {0} does not exist", roleId);
            throw new NotFoundRoleException("Role does not exist");
        }

        return role;
    }
}
