using Senswave.Homes.Domain;
using Senswave.Homes.Domain.Sharings.Services;
using System.Security.Cryptography;

namespace Senswave.Homes.Application.Sharings.Services;

public class PasswordGeneratorService(IOptionsSnapshot<HomeModuleOptions> options) : ISharingPasswordGeneratorService
{
    public string GeneratePassword()
    {
        var passwordLength = options.Value.Sharings.PasswordLength;

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, passwordLength)
            .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
    }
}