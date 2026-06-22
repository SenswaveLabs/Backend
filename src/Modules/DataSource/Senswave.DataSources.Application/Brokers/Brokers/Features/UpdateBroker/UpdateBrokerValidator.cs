using Senswave.Abstractions.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.Infrastructure.Validation;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.UpdateBroker;

public class UpdateBrokerValidator : AbstractValidator<UpdateBrokerCommand>
{
    public UpdateBrokerValidator(IOptions<BrokerOptions> options)
    {
        RuleFor(x => x.Name)
            .StandardCharacterSetWithSpace()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Url)
            .Must(x => Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute))
            .When(x => !string.IsNullOrWhiteSpace(x.Url))
            .MaximumLength(1024)
            .When(x => !string.IsNullOrWhiteSpace(x.Url));

        RuleFor(x => x.ClientName)
            .StandardCharacterSetWithSpace()
            .Length(1, 256)
            .When(x => !string.IsNullOrWhiteSpace(x.ClientName));

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535)
            .When(x => x.Port != int.MaxValue);

        if (options.Value.TestConnectionOnChange)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .StandardCharacterSet()
                .Length(1, 256)
                .When(x => !string.IsNullOrWhiteSpace(x.Url) || !string.IsNullOrWhiteSpace(x.ClientName) || x.Port != int.MaxValue);

            RuleFor(x => x.Password)
                .NotEmpty()
                .StandardCharacterSet()
                .Length(1, 256)
                .When(x => !string.IsNullOrWhiteSpace(x.Url) || !string.IsNullOrWhiteSpace(x.ClientName) || x.Port != int.MaxValue);
        }

        RuleFor(x => x.ProtocolVersion)
            .NotEqual(BrokerProtocolVersion.Invalid);
    }
}
