namespace Senswave.Devices.Application.Operations.Features.DisplayOperations;

public class DisplayOperationsValidator : AbstractValidator<DisplayOperationsQuery>
{
    public DisplayOperationsValidator()
    {
        RuleFor(o => o.UserId)
            .NotEmpty();

        RuleFor(o => o.DeviceId)
            .NotEmpty();

        RuleFor(o => o.Page)
            .GreaterThan(0);

        RuleFor(o => o.Size)
            .GreaterThan(0);
    }
}