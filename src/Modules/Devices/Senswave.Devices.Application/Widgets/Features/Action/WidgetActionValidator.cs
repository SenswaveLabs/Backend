namespace Senswave.Devices.Application.Widgets.Features.Action;

public class WidgetActionValidator : AbstractValidator<WidgetActionCommand>
{
    public WidgetActionValidator()
    {
        RuleFor(x => x.WidgetId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Value)
            .Must(x => x.ToString().Length <= 512)
            .When(x => x.Value is not null);
    }
}
