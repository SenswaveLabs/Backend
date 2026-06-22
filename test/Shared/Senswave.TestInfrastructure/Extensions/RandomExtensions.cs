namespace Senswave.TestInfrastructure.Extensions;

public static class RandomExtensions
{
    public static string RandomString(int length = 20)
    {
        Random random = new();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
