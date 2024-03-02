namespace CleanArchitechture.Application.Common.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{

}

public interface ICommand : IRequest<Result>
{

}
