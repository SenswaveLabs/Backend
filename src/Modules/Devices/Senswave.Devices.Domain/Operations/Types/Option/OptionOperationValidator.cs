using FluentValidation;
using Senswave.Devices.Domain.Operations.Types.Base;
using System.Text.RegularExpressions;

namespace Senswave.Devices.Domain.Operations.Types.Option;

public class OptionOperationValidator : BaseOperationValidator<OptionOperation>
{
    public OptionOperationValidator() : base()
    {
        RuleFor(x => x.Configuration.Options)
                .NotNull()
                .Must(options => options.Count >= 2 && options.Count <= 10)
                .WithMessage("Options must contain between 2 and 10 items.");

        RuleForEach(x => x.Configuration.Options)
            .ChildRules(option =>
            {
                var alphaNumRegex = new Regex(@"^[\p{L}\p{N}\p{M}]+$");

                option.RuleFor(o => o.Name)
                    .NotEmpty()
                    .WithMessage("Option name is required.")
                    .MinimumLength(1)
                    .WithMessage("Option name must be at least 1 character.")
                    .MaximumLength(12)
                    .WithMessage("Option name must be at most 12 characters.")
                    .Matches(@"^[\p{L}\p{N}\p{M} ]+$")
                    .WithMessage("Option name must contain only letters and numbers.");

                option.RuleFor(o => o.Value)
                    .Must(value =>
                    {
                        if (value is null)
                            return false;

                        if (value.TryGetValue<bool>(out _) || value.TryGetValue<double>(out _) || value.TryGetValue<long>(out _))
                            return true;

                        if (value.TryGetValue<string>(out var str))
                        {
                            if (!alphaNumRegex.IsMatch(str))
                                return false;

                            return str.Length <= 64;
                        }

                        return false;
                    })
                    .WithMessage("Option value must be a number, boolean, or alphanumeric string up to 64 bytes.");
            });

        RuleFor(x => x.Configuration.Options)
            .Must(options => options
                .Select(o => o.Name)
                .Distinct()
                .Count() == options.Count)
            .WithMessage("All option names must be unique.");

        RuleFor(x => x.Configuration.Options)
            .Must(options => options
                .Select(o => o.Value?.ToString())
                .Distinct()
                .Count() == options.Count)
            .WithMessage("All option values must be unique.");
    }
}
