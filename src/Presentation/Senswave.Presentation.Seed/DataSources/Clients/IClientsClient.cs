using Refit;
using Senswave.DataSources.Api.Brokers.Clients.StartClient;

namespace Senswave.Presentation.Seed.DataSources.Clients;

public interface IClientsClient
{
    [Post("/v1/datasources/clients/{brokerId}")]
    Task StartClient([Authorize(scheme: "Bearer")] string token, Guid brokerId, StartClientDto dto);
}
