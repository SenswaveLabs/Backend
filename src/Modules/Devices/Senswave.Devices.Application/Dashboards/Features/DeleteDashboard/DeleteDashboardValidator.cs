namespace Senswave.Devices.Application.Dashboards.Features.DeleteDashboard;

public class DeleteDashboardValidator : AbstractValidator<DeleteDashboardCommand>
{
    public DeleteDashboardValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.DashboardId)
            .NotEmpty();
    }
}