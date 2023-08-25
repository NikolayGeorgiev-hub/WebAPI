namespace Application.Services.Models.Users;

public record UserRegistrationRequestModel(string Email, string FirstName, string Password, string ConfirmPassword);


