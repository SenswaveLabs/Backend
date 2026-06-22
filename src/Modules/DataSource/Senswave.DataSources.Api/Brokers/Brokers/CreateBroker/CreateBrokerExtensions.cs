using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Application.Brokers.Brokers.Features.CreateBroker;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;

namespace Senswave.DataSources.Api.Brokers.Brokers.CreateBroker;

public static class CreateBrokerExtensions
{
    public static BrokerCreatedResponse ToBrokerCreatedResponse(this Result<Broker> result) => new()
    {
        Id = result.Data.Id,
    };

    public static CreateBrokerCommand ToCommand(this CreateBrokerRequest dto, Guid userId) => new()
    {
        UserId = userId,
        Name = dto.Name,

        Url = dto.Url,
        ClientName = dto.ClientName,
        Port = dto.Port,
        ProtocolVersion = dto.ProtocolVersion.ToProtocol(),
        UseTls = dto.UseTls,

        Password = dto.Password,
        Username = dto.Username
    };
}
