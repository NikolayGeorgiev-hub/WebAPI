using Application.Services.Models.Roles;

namespace Application.Services.Administration;

public interface IRoleService
{
    Task CreateRoleAsync(RoleRequestModels.Crete requestModel);

    Task EditRoleAsync(Guid roleId, RoleRequestModels.Edit requestModel);

    Task RemoveRoleAsync(Guid roleId);

    Task AssignUsersToRoleAsync(RoleRequestModels.AssignUsers requestModel);

    Task RemoveUsersFromRoleAsync(RoleRequestModels.RemoveUsers requestModel);
}
