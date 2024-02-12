namespace CleanArchitechture.Application.Common.Contracts;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}

public interface ICommand : IRequest
{

}
