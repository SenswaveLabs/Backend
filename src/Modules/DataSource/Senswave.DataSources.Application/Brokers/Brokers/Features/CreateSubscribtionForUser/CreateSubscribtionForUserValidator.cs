using System.Text;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtionForUser;

public class CreateSubscribtionForUserValidator : AbstractValidator<CreateSubscribtionForUserCommand>
{
    public CreateSubscribtionForUserValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.Topic)
            .NotEmpty()
            .Must(x => !x.StartsWith("$"))
            .Must(x => !x.StartsWith("#"))
            .Must(x => !x.StartsWith("+"))
            .Must(x => Encoding.UTF8.GetByteCount(x) >= 1 && Encoding.UTF8.GetByteCount(x) <= 65535)
            .Must(ValidUsageOfHash)
            .Matches(@"^(?!/)(?!.*//)[\P{C}/#+]+$");
    }

    private static bool ValidUsageOfHash(string x)
    {
        var hashCount = x.Count(c => c == '#');

        if (hashCount == 0)
            return true;

        if (hashCount >= 2)
            return false;

        if (x.EndsWith("#"))
            return true;

        return false;
    }
}
