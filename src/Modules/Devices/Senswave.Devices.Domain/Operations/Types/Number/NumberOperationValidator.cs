using FluentValidation;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.Number.DecimalSeparator;

namespace Senswave.Devices.Domain.Operations.Types.Number;

internal class NumberOperationValidator : BaseOperationValidator<NumberOperation>
{
    public NumberOperationValidator() : base()
    {
        RuleFor(x => x.Configuration)
            .NotNull();

        RuleFor(x => x.Configuration.Min)
            .LessThan(x => x.Configuration.Max);

        RuleFor(x => x.Configuration.DecimalSeparator)
            .Must(x => x.ToDecimalSeparator() != DecimalSeparatorType.Invalid);

        RuleFor(x => x.Configuration.DecimalSeparator)
            .Must(x => x.ToDecimalSeparator() != DecimalSeparatorType.Empty);

        RuleFor(x => x.Configuration.DecimalSeparator)
            .Must(x => x.ToDecimalSeparator() == DecimalSeparatorType.Dot)
            .When(x => x.Configuration.IsJson);
    }
}
