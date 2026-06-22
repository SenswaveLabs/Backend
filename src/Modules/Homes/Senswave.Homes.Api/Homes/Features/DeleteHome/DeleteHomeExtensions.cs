using Senswave.Homes.Application.Homes.Features.DeleteHome;

namespace Senswave.Homes.Api.Homes.Features.DeleteHome;

internal static class DeleteHomeExtensions
{
    public static DeleteHomeCommand ToDeleteHomeCommand(this Guid homeId, Guid userId) => new()
    {
        UserId = userId,
        HomeId = homeId,
    };
}
