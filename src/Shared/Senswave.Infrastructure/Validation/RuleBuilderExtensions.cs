using FluentValidation;

namespace Senswave.Infrastructure.Validation;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> StandardCharacterSet<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
        .Matches(@"^[^\s:;'""`]*$");

    public static IRuleBuilderOptions<T, string> StandardCharacterSetWithSpace<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
        .Matches(@"^[^:;'""`]*$");
}
