using Microsoft.AspNetCore.Routing;

namespace Senswave.Infrastructure.Web.Endpoints;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
