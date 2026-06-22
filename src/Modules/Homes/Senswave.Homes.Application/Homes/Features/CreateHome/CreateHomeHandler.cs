using Senswave.Homes.Domain;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.ValueObjects;

namespace Senswave.Homes.Application.Homes.Features.CreateHome;

public class CreateHomeHandler(
    IOptions<HomeModuleOptions> options,
    IHomeQueryRepository queryRepository,
    IHomeCommandRepository repository,
    ILogger<CreateHomeHandler> logger) : ICommandHandler<CreateHomeCommand, Home>
{
    public async Task<Result<Home>> Handle(CreateHomeCommand request, CancellationToken cancellationToken)
    {
        var exists = await repository.HomeExists(request.UserId, request.Name, cancellationToken);

        if (exists)
        {
            logger.LogWarning("[User: {UserId}] Home with name '{Name}' already exists.", request.UserId, request.Name);
            return Result<Home>.Failure(CreateHomeErrors.NameAlreadyUsed);
        }

        Location? location = null;

        if (request.Longitude != double.MinValue && request.Latitude != double.MinValue)
        {
            location = new()
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
        }

        //TODO: Redis Lock per User

        var currentUserHomes = await queryRepository.CountOwnedHomesByUser(request.UserId, cancellationToken);

        if (options.Value.Limits.Homes <= currentUserHomes)
        {
            logger.LogError("[User: {UserId}] Cannot create home, limit reached: {Limit}.", request.UserId, options.Value.Limits.Homes);
            return Result<Home>.Failure(CreateHomeErrors.LimitOfHomesReached);
        }

        var home = new Home
        {
            OwnerId = request.UserId,
            Name = request.Name,
            Icon = request.Icon,

            Location = location,

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await repository.CreateHome(home, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] Failed to create home.", request.UserId);
            return Result<Home>.Failure(result.Errors);
        }

        return Result<Home>.Success(home);
    }
}