using MediatR;
using Senswave.Abstractions.Queries;
using Senswave.Abstractions.Resulting;

namespace Senswave.Abstractions.Handlers;

public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IQuery<TResponse>;
