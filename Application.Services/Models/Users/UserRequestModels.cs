namespace Application.Services.Models.Users;

public static class UserRequestModels
{
    public record Registration(string Email, string FirstName, string Password, string ConfirmPassword);

    public record Login(string Email, string Password);
}
