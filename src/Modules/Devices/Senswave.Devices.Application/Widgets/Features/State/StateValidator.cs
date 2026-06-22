namespace Senswave.Devices.Application.Widgets.Features.State;

public class StateValidator : AbstractValidator<StateCommand>
{
    public StateValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.WidgetId)
            .NotEmpty();
    }
}
