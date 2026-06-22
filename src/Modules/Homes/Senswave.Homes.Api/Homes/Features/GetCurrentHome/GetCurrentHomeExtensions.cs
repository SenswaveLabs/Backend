using Senswave.Abstractions.Resulting;
using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Api.Homes.Features.GetCurrentHome;

public static class GetCurrentHomeExtensionsHomeExtensions
{
    public static GetCurrentHomeResponse ToHomeResponse(this Result<Home> result) => new()
    {
        Id = result.Data.Id,
    };
}