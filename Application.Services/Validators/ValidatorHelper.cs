﻿using System.Text.RegularExpressions;

namespace Application.Services.Validators;

public static class ValidatorHelper
{
    private readonly static Regex regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");

    public static bool ValidateEmailAddress(string email)
    {
        bool result = regex.IsMatch(email);
        return result;
    }
}
