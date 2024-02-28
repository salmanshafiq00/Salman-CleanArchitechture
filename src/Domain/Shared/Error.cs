using System.ComponentModel.DataAnnotations;

namespace CleanArchitechture.Domain.Shared;

public sealed record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType ErrorType { get; }

    public Error(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    public static implicit operator string(Error error) => error?.Code ?? string.Empty;

    //public static implicit operator Result(Error error) => Result.Failure(error);

    public static Error Failure(string code, string description)
        => new(code, description, ErrorType.Failure);

    public static Error Validation(string code, string description)
        => new(code, description, ErrorType.Validation);

    public static Error Unauthorized(string code, string description)
        => new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description)
        => new(code, description, ErrorType.Forbidden);

    public static Error NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description)
        => new(code, description, ErrorType.Conflict);
}

public enum ErrorType
{
    [Display(Name = "Bad Request")]
    Validation = 400,
    [Display(Name = "Bad Request")]
    Failure = 400,
    [Display(Name = "Unauthorized")]
    Unauthorized = 401,
    [Display(Name = "Forbidden")]
    Forbidden = 403,
    [Display(Name = "Not Found")]
    NotFound = 404,
    [Display(Name = "Conflict")]
    Conflict = 409
}
