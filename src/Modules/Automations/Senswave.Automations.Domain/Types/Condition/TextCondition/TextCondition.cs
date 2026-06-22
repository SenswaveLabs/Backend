using Senswave.Automations.Domain.Extensions;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.TextCondition;

public class TextCondition : BaseCondition
{
    [JsonPropertyName("requiredValue")]
    public string RequiredValue { get; set; } = string.Empty;

    public override bool CheckCondition(JsonValue operationPayload)
    {
        if (operationPayload.GetValueKind() != JsonValueKind.String)
            return false;

        var operationValue = operationPayload.GetValue<string>();
        return operationValue == RequiredValue;
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken)
    {
        var validator = new TextConditionValidator();
        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }
}
