using FluentValidation;
using Senswave.Devices.Domain.Widgets.Types.Radio.Model;

namespace Senswave.Devices.Domain.Widgets.Types.Radio;

public class RadioWidgetValidator : AbstractValidator<RadioWidget>
{
    public RadioWidgetValidator() : base()
    {
        RuleFor(x => x.Configuration.Options)
            .NotNull()
            .NotEmpty()
            .WithMessage("Options list must not be empty.")
            .Must(options => options.Count >= 2 && options.Count <= 10)
            .WithMessage("Options list must contain between 2 and 10 items.")
            .Must(HaveUniqueOptionNames)
            .WithMessage("OptionName must be unique across the list.")
            .Must(HaveUniqueDisplayNames)
            .WithMessage("DisplayName must be unique across the list.");

        RuleForEach(x => x.Configuration.Options).ChildRules(option =>
        {
            // DisplayName: 1-12 chars, unique, a-zA-Z0-9 and spaces
            option.RuleFor(x => x.DisplayName)
                .NotEmpty()
                .Length(1, 12).WithMessage("DisplayName must be 1 to 12 characters.")
                .Matches(@"^[\p{L}\p{N}\p{M} -]+$").WithMessage("Value has invalid character.");

            // OptionName: is validated with operation existence - operation itself tests if it valid

            option.RuleFor(x => x.Icon)
                .MaximumLength(AllowedLengths.Icons.MaxLength)
                .Matches(@"^[\p{L}\p{N}\p{M}-]+$")
                .WithMessage("Value has invalid character.");
        });
    }

    private bool HaveUniqueOptionNames(List<RadioOption> options)
    {
        var names = options.Select(x => x.OptionName).ToList();
        return names.Distinct().Count() == names.Count;
    }

    private bool HaveUniqueDisplayNames(List<RadioOption> options)
    {
        var names = options.Select(x => x.DisplayName).ToList();
        return names.Distinct().Count() == names.Count;
    }
}

