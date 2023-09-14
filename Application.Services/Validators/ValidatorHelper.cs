using Application.Services.Models.Users;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Application.Services.Validators;

public static class ValidatorHelper
{
    private readonly static Regex regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");

    public static bool ValidateEmailAddress(string email)
    {
        bool result = regex.IsMatch(email);
        return result;
    }

    public static bool IsValidGuid(Guid currentId)
    {
        string valueToString = currentId.ToString();
        bool result = Guid.TryParse(valueToString, out var isValid);

        return result;
    }

    public static bool IsValidDate(DateTime? date)
    {
        bool result = DateTime.TryParse(date.ToString(), out var isValid);
        return result;
    }
}
