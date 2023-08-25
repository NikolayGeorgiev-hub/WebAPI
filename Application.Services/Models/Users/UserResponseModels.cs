namespace Application.Services.Models.Users;

public static class UserResponseModels
{
    public record Profile(string FirstName, string Email, string PhoneNumber);
}
