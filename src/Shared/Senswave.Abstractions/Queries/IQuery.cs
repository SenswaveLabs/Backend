using MediatR;
using Senswave.Abstractions.Resulting;

namespace Senswave.Abstractions.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
