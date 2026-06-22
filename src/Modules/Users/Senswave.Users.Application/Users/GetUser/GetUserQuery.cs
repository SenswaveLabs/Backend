namespace Senswave.Users.Application.Users.GetUser;

public class GetUserQuery : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
