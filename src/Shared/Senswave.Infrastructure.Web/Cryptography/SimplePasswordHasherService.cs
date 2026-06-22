using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Senswave.Abstractions.Cryptography;

namespace Senswave.Infrastructure.Web.Cryptography;

internal class SimplePasswordHasherService(
    IOptions<PasswordHasherOptions> options) : IPasswordHashingService
{
    private readonly PasswordHasher<object> hasher = new(options);

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        return hasher.HashPassword(null, password);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return false;

        if (string.IsNullOrEmpty(password))
            return false;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var result = hasher.VerifyHashedPassword(null, hashedPassword, password);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
