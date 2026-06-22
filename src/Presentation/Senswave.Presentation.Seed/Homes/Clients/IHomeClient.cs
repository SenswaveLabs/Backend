using Refit;
using Senswave.Homes.Api.Homes.Features.CreateHome;
using Senswave.Homes.Api.Homes.Features.SetHomeDataSource;

namespace Senswave.Presentation.Seed.Homes.Clients;

public interface IHomeClient
{
    [Post("/v1/homes")]
    Task<HomeCreatedResponse> CreateHome([Authorize(scheme: "Bearer")] string token, CreateHomeRequest home);

    [Put("/v1/homes/{homeId}/datasource")]
    Task AssignBrokerToHome([Authorize(scheme: "Bearer")] string token, Guid homeId, [Body] AssignHomeDataSourceRequest request);
}
