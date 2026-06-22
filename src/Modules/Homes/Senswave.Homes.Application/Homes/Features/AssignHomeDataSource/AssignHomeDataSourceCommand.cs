namespace Senswave.Homes.Application.Homes.Features.AssignHomeDataSource;

public class AssignHomeDataSourceCommand : ICommand
{
    public Guid UserId { get; set; }
    public Guid HomeId { get; set; }
    public Guid DataSourceId { get; set; }
}
