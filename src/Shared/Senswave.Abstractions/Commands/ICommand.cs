using MediatR;
using Senswave.Abstractions.Resulting;

namespace Senswave.Abstractions.Commands;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TRequest> : IRequest<Result<TRequest>>
{
}
