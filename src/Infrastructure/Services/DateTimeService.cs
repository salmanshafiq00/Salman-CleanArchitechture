using CleanArchitechture.Application.Common.Abstractions;

namespace CleanArchitechture.Infrastructure.Services;

internal class DateTimeService : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
