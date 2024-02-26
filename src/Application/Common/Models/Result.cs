using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Enums;

namespace CleanArchitechture.Application.Common.Models;

public class Result
{
    protected internal Result(bool isSucceed, params string[] errors)
    {
        if (isSucceed && errors.Length != 0
            || !isSucceed && errors.Length == 0)
        {
            throw new ArgumentException("Invalid error", nameof(errors));
        }
        IsSucceed = isSucceed;
        if (isSucceed) 
        { 
            StatusCode = 200;
            MessageType = MessageType.Success;
        }
        if (!isSucceed)
        {
            MessageType = MessageType.Error;
        }
        Errors = errors;
    }
    protected internal Result(bool isSucceed, string message, MessageType messageType)
        : this(isSucceed, [])
    {
        Message = message;
        MessageType = messageType;
        StatusCode = 200;
    }

    protected internal Result(bool isSucceed, ErrorType errorType, params string[] errors)
       : this(isSucceed, errors)
    {
        ErrorType = errorType;
        StatusCode = (int)errorType;
        MessageType = MessageType.Error;
    }

    public bool IsSucceed { get; }

    [JsonIgnore]
    public bool IsFailed => !IsSucceed;

    public string[] Errors { get; }

    public string Message { get; init; } = string.Empty;
    public int StatusCode { get; init; }

    public MessageType MessageType { get; }

    [JsonIgnore]
    public ErrorType ErrorType { get; }

    public static Result Success() => new(true, string.Empty, MessageType.Success);
    public static Result Success(string message) => new(true, message, MessageType.Success);
    public static Result Success(string message, MessageType messageType) => new(true, message, messageType);


    public static Result Failure(params string[] errors) => new(false, ErrorType.Failure, errors);
    public static Result Validation(params string[] errors) => new(false, ErrorType.Validation, errors);
    public static Result NotFound(params string[] errors) => new(false, ErrorType.NotFound, errors);
    public static Result Conflict(params string[] errors) => new(false, ErrorType.Conflict, errors);

    #region Result<TValue> Static Factory Method

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, []);
    public static Result<TValue> Success<TValue>(TValue value, string message) => new(value, true, message, MessageType.Success);

    public static Result<TValue> Failure<TValue>(params string[] errors) => new(default, false, ErrorType.Failure, errors);
    public static Result<TValue> Validation<TValue>(params string[] errors) => new(default, false, ErrorType.Validation, errors);
    public static Result<TValue> NotFound<TValue>(params string[] errors) => new(default, false, ErrorType.NotFound, errors);
    public static Result<TValue> Conflict<TValue>(params string[] errors) => new(default, false, ErrorType.Conflict, errors);


    #endregion


    //public static implicit operator Result(string[] errors) => Result.Failure(errors);
}

public class Result<TValue> : Result
{
    public Result(TValue? value, bool isSucceed, params string[] errors)
        : base(isSucceed, errors)
    {
        Value = value;
    }

    public Result(TValue? value, bool isSucceed, string message, MessageType messageType)
        : base(isSucceed, message, messageType)
    {
        Value = value;
    }

    public Result(TValue? value, bool isSucceed, ErrorType errorType, params string[] errors)
        : base(isSucceed, errorType, errors)
    {
        Value = value;
    }

    public TValue? Value { get; init; }

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    // public static implicit operator Result<TValue>(string[] errors) => Failure<TValue>(errors);

}
