using Senswave.Homes.Application.Homes.Features.UpdateHome;

namespace Senswave.Homes.Api.Homes.Features.UpdateHome;

public static class UpdateHomeExtensions
{
    public static UpdateHomeCommand ToCommand(this UpdateHomeRequest patchHomeDto, Guid userId, Guid homeId)
    {
        var patchHomeCommand = new UpdateHomeCommand()
        {
            HomeId = homeId,
            UserId = userId,

            Name = patchHomeDto.Name,
            Icon = patchHomeDto.Icon,
        };

        if (patchHomeDto.Latitude.HasValue)
            patchHomeCommand.Latitude = patchHomeDto.Latitude.Value;

        if (patchHomeDto.Longitude.HasValue)
            patchHomeCommand.Longitude = patchHomeDto.Longitude.Value;

        return patchHomeCommand;
    }
}