using Senswave.Abstractions.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Infrastructure.Validation;

namespace Senswave.Devices.Application.Devices.Features.UpdateDevice;

public class UpdateDeviceValidator : AbstractValidator<UpdateDeviceCommand>
{
    public UpdateDeviceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Empty()
            .When(x => string.IsNullOrWhiteSpace(x.Name))
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .StandardCharacterSetWithSpace()
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Icon)
            .StandardCharacterSet()
            .MaximumLength(AllowedLengths.Icons.MaxLength);

        RuleFor(x => x.TileType)
            .NotEqual(DeviceTileType.Invalid);

        RuleFor(x => x.PresenceType)
            .NotEqual(DevicePresenceType.Invalid);
    }
}