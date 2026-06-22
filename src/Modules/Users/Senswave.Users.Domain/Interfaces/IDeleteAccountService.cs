namespace Senswave.Users.Domain.Interfaces;

public interface IDeleteAccountService
{
    Task DeleteAccountAsync(string email);
}
