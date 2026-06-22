using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Application.Homes.Features.UpdateHome;

public class UpdateHomeCommand : ICommand<Home>
{
    public Guid HomeId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public bool RemoveLocalization { get; set; }

    public double Latitude { get; set; } = double.MinValue;

    public double Longitude { get; set; } = double.MinValue;
}