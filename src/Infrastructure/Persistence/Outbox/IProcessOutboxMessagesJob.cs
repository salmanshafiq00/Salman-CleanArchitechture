namespace CleanArchitechture.Infrastructure.Persistence.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessOutboxMessagesAsync();
}
