using Refit;
using Senswave.DataSources.Api.Brokers.Brokers.CreateBroker;
using Senswave.DataSources.Api.Brokers.Brokers.GetBrokers;

namespace Senswave.Presentation.Seed.DataSources.Clients;

public interface IBrokerClient
{
    [Post("/v1/datasources/brokers")]
    Task<BrokerCreatedResponse> CreateBroker([Authorize(scheme: "Bearer")] string token, CreateBrokerRequest dto);

    [Get("/v1/datasources/brokers")]
    Task<GetBrokersResponse> GetBrokers([Authorize(scheme: "Bearer")] string token);
}
