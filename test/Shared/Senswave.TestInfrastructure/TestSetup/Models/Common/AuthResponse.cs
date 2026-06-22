namespace Senswave.TestInfrastructure.TestSetup.Models.Common;

public class AuthResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = string.Empty;
}
