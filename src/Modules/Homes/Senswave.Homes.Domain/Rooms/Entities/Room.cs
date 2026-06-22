using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Domain.Rooms.Entities;

public class Room : Entity
{
    public Guid HomeId { get; set; }

    public Home Home { get; set; }

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; }
}
