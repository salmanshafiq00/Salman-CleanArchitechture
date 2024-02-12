namespace CleanArchitechture.Application.Common.Contracts;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
