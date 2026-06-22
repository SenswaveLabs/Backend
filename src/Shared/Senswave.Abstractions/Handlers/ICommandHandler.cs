using MediatR;
using Senswave.Abstractions.Commands;
using Senswave.Abstractions.Resulting;

namespace Senswave.Abstractions.Handlers;

public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : ICommand<TResponse>;

public interface ICommandHandler<TRequest> : IRequestHandler<TRequest, Result>
    where TRequest : ICommand;