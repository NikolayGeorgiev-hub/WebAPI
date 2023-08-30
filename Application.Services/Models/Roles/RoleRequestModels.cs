namespace Application.Services.Models.Roles;

public static class RoleRequestModels
{
    public record Crete(string Name);

    public record Edit(string Name);

    public record AssignUsers(IList<Guid> UsersId, Guid RoleId);

    public record RemoveUsers(IList<Guid> UsersId, Guid RoleId);
}
