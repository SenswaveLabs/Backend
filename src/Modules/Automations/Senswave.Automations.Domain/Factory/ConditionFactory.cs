using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Enums;
using Senswave.Automations.Domain.Types.Condition;
using Senswave.Automations.Domain.Types.Condition.BooleanCondition;
using Senswave.Automations.Domain.Types.Condition.NumericCondition;
using Senswave.Automations.Domain.Types.Condition.TextCondition;

namespace Senswave.Automations.Domain.Factory;

public sealed class ConditionFactory
{
    public async Task<Result<BaseCondition>> Create(AutomationCondition automationCondition, CancellationToken cancellationToken)
    {
        try
        {
            var condition = automationCondition.ConditionType switch
            {
                AutomationConditionType.BooleanCondition => Result<BaseCondition>.Success(automationCondition.ToBooleanCondition()),
                AutomationConditionType.NumberCondition => Result<BaseCondition>.Success(automationCondition.ToNumericCondition()),
                AutomationConditionType.TextCondition => Result<BaseCondition>.Success(automationCondition.ToTextCondition()),
                _ => Result<BaseCondition>.Failure(Error.Failure("InvalidAutomationConditionType", "Cannot create automation condition implementation."))
            };

            if (condition.IsFailure)
                return Result<BaseCondition>.Failure(Error.Failure("AutomationConditionCreationError", $"An error occurred while creating the automation condition"));

            condition.Data.OperationId = automationCondition.OperationId;
            var isValidationOk = await condition.Data.Validate(cancellationToken);

            if (isValidationOk.IsFailure)
                return Result<BaseCondition>.Failure(Error.Failure("AutomationConditionValidationError", "The created automation condition is not valid."));

            return condition;
        }
        catch (Exception ex)
        {
            return Result<BaseCondition>.Failure(Error.Failure("AutomationConditionCreationError", $"An error occurred while creating the automation condition: {ex.Message}"));
        }
    }

    public async Task<IList<Result<BaseCondition>>> Create(IList<AutomationCondition> automationConditions, CancellationToken cancellationToken)
    {
        var results = new List<Result<BaseCondition>>();

        foreach (var automationCondition in automationConditions)
        {
            var result = await Create(automationCondition, cancellationToken);
            results.Add(result);
        }

        return results;
    }


}