using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;

namespace Senswave.Homes.Application.Homes.Features.DeleteHome;

public class DeleteHomeHandler(
    IHomeAccessService accessService,
    IHomeCommandRepository repository,
    ILogger<DeleteHomeHandler> logger) : ICommandHandler<DeleteHomeCommand>
{
    public async Task<Result> Handle(DeleteHomeCommand request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(request.UserId, request.HomeId, cancellationToken);

        if (!isOwner)
            return Result.Failure(isOwner.Errors);

        var result = await repository.RemoveHome(request.HomeId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to delete home.", request.UserId, request.HomeId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] [Home: {HomeId}] Home deleted successfully.", request.UserId, request.HomeId);
        return Result.Success();
    }
}