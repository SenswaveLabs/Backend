using Senswave.Automations.Domain.Extensions;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.BooleanCondition;

public class BooleanCondition : BaseCondition
{
    // IsOn value tells if operation should be on or off
    [JsonPropertyName("isOn")]
    public bool? IsOn { get; set; }

    public override bool CheckCondition(JsonValue operationPayload)
    {
        // In this stage it should not be null. It just another bullet-prof
        if (IsOn == null)
            return false;

        if (operationPayload.GetValueKind() != JsonValueKind.True
            && operationPayload.GetValueKind() != JsonValueKind.False)
            return false;

        var operationValue = operationPayload.GetValue<bool>();
        if (operationValue == IsOn.Value)
            return true;

        return false;
    }
    public override async Task<Result> Validate(CancellationToken cancellationToken)
    {
        var validator = new BooleanConditionValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

}
