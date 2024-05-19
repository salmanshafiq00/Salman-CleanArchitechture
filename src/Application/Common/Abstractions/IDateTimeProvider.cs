namespace CleanArchitechture.Application.Common.Abstractions;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}
