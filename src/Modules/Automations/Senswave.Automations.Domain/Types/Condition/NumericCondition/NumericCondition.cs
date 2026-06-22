using Senswave.Automations.Domain.Extensions;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.NumericCondition;

public class NumericCondition : BaseCondition
{
    [JsonPropertyName("minValue")]
    public double? Min { get; set; }

    [JsonPropertyName("maxValue")]
    public double? Max { get; set; }

    public override bool CheckCondition(JsonValue operationPayload)
    {
        if (!Min.HasValue)
            Min = Double.NegativeInfinity;

        if (!Max.HasValue)
            Max = Double.PositiveInfinity;

        if (operationPayload.GetValueKind() != JsonValueKind.Number)
            return false;

        var result = operationPayload.GetValue<double>();

        if (Min <= result && result <= Max)
            return true;

        return false;
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken)
    {
        var validator = new NumericConditionValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }
}
