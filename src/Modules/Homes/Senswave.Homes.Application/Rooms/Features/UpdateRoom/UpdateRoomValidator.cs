using Senswave.Infrastructure.Validation;

namespace Senswave.Homes.Application.Rooms.Features.UpdateRoom;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.Name)
            .Empty()
            .When(x => string.IsNullOrWhiteSpace(x.Name))
            .Length(3, 64)
            .StandardCharacterSetWithSpace()
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.RoomId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
