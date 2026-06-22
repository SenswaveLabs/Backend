namespace Senswave.Homes.Application.Homes.Features.DeleteHome;

public class DeleteHomeCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;
}