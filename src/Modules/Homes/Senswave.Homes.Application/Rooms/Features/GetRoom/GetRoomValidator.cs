namespace Senswave.Homes.Application.Rooms.Features.GetRoom;

public class GetRoomValidator : AbstractValidator<GetRoomQuery>
{
    public GetRoomValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.RoomId)
            .NotEmpty();
    }
}
