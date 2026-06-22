using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Homes.ValueObjects;

namespace Senswave.Homes.Application.Homes.Features.UpdateHome;

public class UpdateHomeHandler(
    IHomeAccessService accessService,
    IHomeCommandRepository repository,
    ILogger<UpdateHomeHandler> logger) : ICommandHandler<UpdateHomeCommand, Home>
{
    public async Task<Result<Home>> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManage(request.UserId, request.HomeId, cancellationToken);

        if (!canManage)
            return Result<Home>.Failure(canManage.Errors);

        var home = await repository.GetHome(request.HomeId, cancellationToken);

        if (home == null)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Home not found.", request.UserId, request.HomeId);
            return Result<Home>.Failure(UpdateHomeError.HomeNotFound);
        }

        if (!string.IsNullOrEmpty(request.Name) && request.Name != home.Name)
        {
            var homeWithNameExists = await repository.HomeExists(request.UserId, request.Name, cancellationToken);

            if (homeWithNameExists)
            {
                logger.LogWarning("[User: {UserId}] [Home: {HomeId}] Home with name '{Name}' already exists.",
                    request.UserId,
                    request.HomeId,
                    request.Name);

                return Result<Home>.Failure(UpdateHomeError.HomeNameUsed);
            }

            home.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Icon))
            home.Icon = request.Icon;

        UpdateLocation(home, request);

        var result = await repository.UpdateHome(home, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] [Home: {HomeId}] Failed to update home.",
                request.UserId,
                request.HomeId);

            return Result<Home>.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] [Home: {HomeId}] Home updated successfully.",
            request.UserId,
            request.HomeId);

        return Result<Home>.Success(home);
    }

    private void UpdateLocation(Home home, UpdateHomeCommand request)
    {
        if (request.RemoveLocalization)
        {
            home.Location = null;

            logger.LogInformation("[Home: {homeId}] Removed location for home.", home.Id);
            return;
        }

        if (home.Location == null && request.Longitude != double.MinValue && request.Latitude != double.MaxValue)
        {

            home.Location = new Location()
            {
                Latitude = request.Longitude,
                Longitude = request.Latitude,
                Home = home
            };

            logger.LogInformation("[Home: {homeId}] Created location for home.",
                home.Id);

            return;
        }

        if (home.Location == null)
            return;

        if (request.Longitude != double.MinValue && request.Longitude != home.Location.Longitude)
        {
            logger.LogInformation("[Home: {homeId}] Updated longitude for home.",
                home.Id);

            home.Location.Longitude = request.Longitude;
        }

        if (request.Latitude != double.MinValue && request.Latitude != home.Location.Latitude)
        {
            logger.LogInformation("[Home: {homeId}] Updated latitude for home.",
                home.Id);

            home.Location.Latitude = request.Latitude;
        }
    }
}