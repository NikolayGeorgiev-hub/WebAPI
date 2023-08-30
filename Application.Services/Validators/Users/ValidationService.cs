using Application.Common.Exceptions;
using Application.Common.Resources;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Application.Services.Validators.Users;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ValidationService> logger;

    public ValidationService(IServiceProvider serviceProvider, ILogger<ValidationService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public void Validate<T>(T model)
    {
        if (model is null)
        {
            //throw Exception
        }

        object? validatorObject = this.serviceProvider.GetService(typeof(IValidator<T>));

        if (validatorObject is null)
        {
            this.logger.LogWarning("No registered validator for the type {Type}", typeof(IValidator<T>).FullName);
           // throw new ServiceNotConfiguredException(Messages.GeneralErrorMessage);
        }

        IValidator<T> validator = (IValidator<T>)validatorObject;
        ValidationResult result = validator.Validate(model);

        if (!result.IsValid)
        {
            string[] errorMessages = result.Errors
                .Select(x => x.ErrorMessage)
                .ToArray();

            string message = string.Join(",", errorMessages);
            throw new InvalidModelStateException(message, errorMessages);
        }
    }
}
