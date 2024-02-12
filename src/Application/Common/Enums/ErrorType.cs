using System.ComponentModel.DataAnnotations;

namespace CleanArchitechture.Application.Common.Enums;

public enum ErrorType
{
    [Display(Name = "Bad Request")]
    Validation = 400,
    [Display(Name = "Unauthorized")]
    Unauthorized = 401,
    [Display(Name = "Forbidden")]
    Forbidden = 403,
    [Display(Name = "Not Found")]
    NotFound = 404,
    [Display(Name = "Conflict")]
    Conflict = 409,
    [Display(Name = "Internal Server Error")]
    Failure = 500
}

public static class ErrorTypeExtensions
{
    public static string GetDisplayName(this ErrorType errorType)
    {
        DisplayAttribute? displayAtrribute = errorType.GetType()
            .GetField(errorType.ToString())
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .FirstOrDefault() as DisplayAttribute;

        return displayAtrribute?.Name ?? nameof(errorType);
    }
}

