using Senswave.Abstractions.Resulting;
using Senswave.Homes.Application.Homes.Features.CreateHome;
using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Api.Homes.Features.CreateHome;

public static class CreateHomeExtensions
{
    public static HomeCreatedResponse ToCreatedResponse(this Result<Home> result) => new()
    {
        Id = result.Data.Id
    };

    public static CreateHomeCommand ToCommand(this CreateHomeRequest request, Guid userId)
    {
        var postHomeCommand = new CreateHomeCommand()
        {
            UserId = userId,
            Name = request.Name,
            Icon = request.Icon,
        };

        if (request.Latitude.HasValue)
            postHomeCommand.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            postHomeCommand.Longitude = request.Longitude.Value;

        return postHomeCommand;
    }
}