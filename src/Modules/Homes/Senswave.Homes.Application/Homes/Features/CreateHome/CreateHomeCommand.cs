using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Application.Homes.Features.CreateHome;

public class CreateHomeCommand : ICommand<Home>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;


    public double Latitude { get; set; } = double.MinValue;

    public double Longitude { get; set; } = double.MinValue;

}