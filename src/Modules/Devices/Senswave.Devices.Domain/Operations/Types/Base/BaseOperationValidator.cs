using FluentValidation;
using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.Domain.Operations.Types.Base;

public class BaseOperationValidator<T> : AbstractValidator<T> where T : BaseOperation
{
    public BaseOperationValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        RuleFor(x => x.Type)
            .NotEqual(OperationType.Invalid)
            .NotEqual(OperationType.Empty);

        RuleFor(x => x.Configuration.JsonNames)
            .Empty()
            .When(x => !x.Configuration.IsJson);

        RuleFor(x => x.Configuration.JsonNames)
            .Must(x => x.Length > 0 && x.Length <= 5)
            .When(x => x.Configuration.IsJson);

        RuleForEach(x => x.Configuration.JsonNames)
            .Must(x => x.Length > 0)
            .ChildRules(names =>
            {
                names.RuleFor(name => name)
                     .NotEmpty()
                     .Length(1, 64);
            })
            .When(x => x.Configuration.IsJson);

        RuleFor(x => x.Configuration.SaveOnUserAction)
            .Must(x => x == true || x == false);
    }
}
