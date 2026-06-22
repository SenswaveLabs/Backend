using FluentValidation;
using Senswave.Devices.Domain.Operations.Types.Base;

namespace Senswave.Devices.Domain.Operations.Types.Integer;

public class IntegerOperationValidator : BaseOperationValidator<IntegerOperation>
{
    public IntegerOperationValidator() : base()
    {
        RuleFor(x => x.Configuration)
            .NotNull();

        RuleFor(x => x.Configuration.Min)
            .LessThanOrEqualTo(x => x.Configuration.Max);
    }
}
