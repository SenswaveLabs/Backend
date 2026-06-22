using Microsoft.AspNetCore.Http;
using Senswave.Abstractions.Contexts;
using System.Security.Claims;

namespace Senswave.Infrastructure.Web.Contexts;

public class UserRequestContext(IHttpContextAccessor contextAccessor) : IRequestContext
{
    public Guid UserId
    {
        get
        {
            var userId = contextAccessor!.
                HttpContext?.
                User?.
                FindFirst(ClaimTypes.NameIdentifier)?.
                Value!;

            if (Guid.TryParse(userId, out var guidUserId))
                return guidUserId;

            return Guid.Empty;
        }
    }
}
