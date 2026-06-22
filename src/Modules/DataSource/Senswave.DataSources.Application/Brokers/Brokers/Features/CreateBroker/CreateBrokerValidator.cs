using Senswave.Abstractions.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.Infrastructure.Validation;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateBroker;

public class CreateBrokerValidator : AbstractValidator<CreateBrokerCommand>
{
    public CreateBrokerValidator(IOptions<BrokerOptions> options)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .StandardCharacterSetWithSpace()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(x => Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute))
            .MaximumLength(1024);

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .StandardCharacterSetWithSpace()
            .Length(1, 256);

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535);

        if (options.Value.TestConnectionOnChange)
        {
            RuleFor(x => x.Username)
                .StandardCharacterSet()
                .NotEmpty()
                .Length(1, 256);

            RuleFor(x => x.Password)
                .StandardCharacterSet()
                .NotEmpty()
                .Length(1, 256);
        }

        RuleFor(x => x.ProtocolVersion)
            .NotEqual(BrokerProtocolVersion.Invalid)
            .NotEqual(BrokerProtocolVersion.Empty);
    }
}
