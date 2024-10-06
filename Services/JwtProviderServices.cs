using System;
using System.Text.Json.Serialization;

namespace AoL_HCI_Backend.Services;

public interface IJwtProviderServices{
    public Task<AuthToken?> GenerateToken(string email, string password);
}

internal sealed class JwtProviderService(HttpClient httpClient) : IJwtProviderServices
{
    private readonly HttpClient _httpclient = httpClient;

    public async Task<AuthToken?> GenerateToken(string email, string password){
        var request = new {
            email,
            password,
            returnSecureToken = true,
        };
        var response = await _httpclient.PostAsJsonAsync("", request);
        var token = await response.Content.ReadFromJsonAsync<AuthToken>();
        return token;
    }
}

public record AuthToken{
    [JsonPropertyName("IdToken")]
    public string IdToken { get; set; } = null!;
    [JsonPropertyName("LocalId")]
    public string IdentityId { get; set; } = null!;
    [JsonPropertyName("RefreshToken")]
    public string RefreshToken { get; set; } = null!;
}