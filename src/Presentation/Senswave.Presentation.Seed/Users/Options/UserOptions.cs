using Senswave.Presentation.Seed.Automations.Options;
using Senswave.Presentation.Seed.DataSources.Options;
using Senswave.Presentation.Seed.Devices.Options;
using Senswave.Presentation.Seed.Homes.Options;
using Senswave.Users.Domain.ValueObjects;

namespace Senswave.Presentation.Seed.Users.Options;

public class UserOptions
{
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RoleTypes UserRole => Role switch
    {
        "admin" => RoleTypes.Admin,
        _ => RoleTypes.User
    };
    public string Role { get; set; } = string.Empty;

    public AutomationOptions Automations { get; set; } = new();
    public BrokerOptions Broker { get; set; } = new();
    public HomeOptions Home { get; set; } = new();
    public DeviceOptions Devices { get; set; } = new();
}
