namespace Application.Services.Validators;

public interface IValidationService
{
    void Validate<T>(T model);
}
