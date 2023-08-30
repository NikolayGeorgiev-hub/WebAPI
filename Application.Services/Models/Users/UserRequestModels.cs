namespace Application.Services.Models.Users;

public static class UserRequestModels
{
    public record Registration(string Email, string FirstName, string Password, string ConfirmPassword);

    public record Login(string Email, string Password);

    public record ConfirmEmail(string Email);

    public record IdentityToken(string Token, string Email);

    public record ForgetPassword(string Email);

    public record ResetPassword(string Password, string ConfirmPassword);
}
