namespace Senswave.Devices.Application.Widgets.Features.DeleteWidget;

public class DeleteWidgetValidator : AbstractValidator<DeleteWidgetCommand>
{
    public DeleteWidgetValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();
        RuleFor(x => x.WidgetId)
            .NotEmpty();
    }
}