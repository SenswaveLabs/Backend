namespace Senswave.Homes.Application.Homes.Features.DeleteHomeDataSource;

public class DeleteHomeDataSourceCommand : ICommand
{
    public Guid UserId { get; set; }
    public Guid HomeId { get; set; }
}
