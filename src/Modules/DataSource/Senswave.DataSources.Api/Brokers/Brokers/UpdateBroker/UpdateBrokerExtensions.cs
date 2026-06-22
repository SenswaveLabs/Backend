using Senswave.DataSources.Application.Brokers.Brokers.Features.UpdateBroker;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;

namespace Senswave.DataSources.Api.Brokers.Brokers.UpdateBroker;

internal static class UpdateBrokerExtensions
{
    internal static UpdateBrokerCommand ToCommand(this UpdateBrokerRequest dto, Guid userId, Guid brokerId)
    {
        var patchBrokerCommand = new UpdateBrokerCommand()
        {
            UserId = userId,
            BrokerId = brokerId,
            Name = dto.Name,

            Url = dto.Url,
            ClientName = dto.ClientName,
            ProtocolVersion = dto.ProtocolVersion.ToProtocol(),

            Password = dto.Password,
            Username = dto.Username
        };

        if (dto.Port.HasValue && dto.Port != 0)
            patchBrokerCommand.Port = dto.Port.Value;

        if (dto.UseTls.HasValue)
            patchBrokerCommand.UseTls = dto.UseTls.Value;

        return patchBrokerCommand;
    }
}
