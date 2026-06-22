using Senswave.Infrastructure.Validation;

namespace Senswave.Homes.Application.Rooms.Features.CreateRoom;

public class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .StandardCharacterSetWithSpace()
            .Length(3, 64);
    }
}
