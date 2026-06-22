using Senswave.Automations.Application.Features.DeleteResult;

namespace Senswave.Automations.Api.Features.DeleteResult;

internal static class DeleteResultExtension
{
    public static DeleteResultCommand ToDeleteCommand(this Guid resultId, Guid userId) => new()
    {
        ResultId = resultId,
        UserId = userId
    };
}