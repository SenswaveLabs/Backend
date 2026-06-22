using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Persistence;
using Senswave.Users.Domain.ValueObjects;
using Senswave.Users.Infrastructure;

namespace Senswave.Users.Api;

public class RoleInitializer(UsersContext dbContext,
    RoleManager<IdentityRole<Guid>> roleManager,
    ILogger<RoleInitializer> logger)
    : IDatabaseInitializer
{
    public async Task Initialize()
    {
        await AddRolesAsync();
        await dbContext.SaveChangesAsync();
    }

    private async Task AddRolesAsync()
    {
        logger.LogDebug("Initializing roles.");
        await AddRole(RoleTypes.Admin);
        await AddRole(RoleTypes.User);
        logger.LogDebug("Initialized roles.");
    }

    private async Task AddRole(RoleTypes roleData)
    {
        var roleExists = await roleManager.FindByNameAsync(roleData.NormalizedName);

        if (roleExists is not null)
        {
            logger.LogInformation("Role {role} already exists.", roleData.Name);
            return;
        }

        await dbContext.Roles.AddAsync(new IdentityRole<Guid>
        {
            Name = roleData.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            Id = Guid.CreateVersion7(),
            NormalizedName = roleData.NormalizedName,
        });

        await dbContext.SaveChangesAsync();
    }
}
