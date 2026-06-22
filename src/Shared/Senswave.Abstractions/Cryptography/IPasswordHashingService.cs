namespace Senswave.Abstractions.Cryptography;

public interface IPasswordHashingService
{
    /// <summary>
    /// Hashes the provided password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password to verify against.</param>
    /// <param name="password">The plain text password to verify.</param>
    /// <returns>True if the password matches the hashed password, otherwise false.</returns>
    bool VerifyPassword(string hashedPassword, string password);
}
