using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtionForUser;

namespace Senswave.DataSources.Api.Brokers.Brokers.CreateSubscribtion;

public static class CreateSubscribtionExtensions
{
    public static CreateSubscribtionResponse ToResponse(this Result<Guid> result) => new()
    {
        Id = result.Data
    };

    public static CreateSubscribtionForUserCommand ToCommand(this CreateSubscribtionRequest request, Guid brokerId, Guid userId) => new()
    {
        UserId = userId,
        BrokerId = brokerId,
        Topic = request.Topic
    };
}
