using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.TestInfrastructure.TestSetup.Models.Users;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.ValueObjects;

namespace Senswave.TestInfrastructure.TestSetup.Initializers;

public class TestUsersInitializer(IServiceScope scope)
{
    private readonly UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    private readonly static TestUser[] TestUsers =
    [
        new TestUser
        {
            Email = "admin@gmail.com",
            Password = "Admin123456!",
            Role = RoleTypes.Admin
        },
        new TestUser
        {
            Email = "user@gmail.com",
            Password = "User123456!",
            Role = RoleTypes.User
        },
        new TestUser
        {

            Email = "intruder@gmail.com",
            Password = "Intruder123!",
            Role = RoleTypes.User
        }
    ];

    public async Task InitializeUsers(TestUser[]? additionalUsers = null)
    {
        var testUsers = TestUsers;

        if (additionalUsers != null)
            testUsers = testUsers.Concat(additionalUsers).ToArray();

        foreach (var user in testUsers)
        {
            await AddUser(user.Email, user.Password, user.Role);
        }
    }

    private readonly SemaphoreSlim semaphore = new(1, 1);

    private async Task AddUser(string email, string password, RoleTypes role)
    {
        await semaphore.WaitAsync();

        try
        {
            var userExists = await userManager.Users.AnyAsync(x => x.UserName == email);

            if (userExists)
                return;

            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role.NormalizedName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user {email}: {ex.Message}");


            var userExists = await userManager.Users.AnyAsync(x => x.UserName == email);

            if (userExists)
            {
                Console.WriteLine($"User {email} already exists, skipping creation.");
            }

        }
        finally
        {
            semaphore.Release();
        }
    }
}
