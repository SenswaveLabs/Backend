using Senswave.Integration.Shared;

namespace Senswave.Integration.Homes.HomeUsers;

public record HomeUsersResponse : BaseInternalResponse
{
    public List<HomeUser> Users { get; set; } = [];

    public record HomeUser
    {
        public Guid UserId { get; set; }

        public string HomeSharingType { get; set; } = string.Empty;
    }
}
