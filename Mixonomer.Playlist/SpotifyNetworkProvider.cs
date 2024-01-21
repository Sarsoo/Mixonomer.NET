using Google.Cloud.SecretManager.V1;
using Mixonomer.Fire;
using SpotifyAPI.Web;

namespace Mixonomer.Playlist;

public class SpotifyNetworkProvider
{
    private readonly SecretManagerServiceClient _secretClient;
    private readonly UserRepo _userRepo;

    public SpotifyNetworkProvider(UserRepo userRepo, SecretManagerServiceClient secretClient)
    {
        _userRepo = userRepo;
        _secretClient = secretClient ?? SecretManagerServiceClient.Create();
    }

    public async Task<SpotifyClientConfig> GetUserConfig(string username)
    {
        return await GetUserConfig(await _userRepo.GetUser(username));
    }

    public async Task<SpotifyClientConfig> GetUserConfig(User user)
    {
        var spotifyClient = await _secretClient.AccessSecretVersionAsync(SecretStrings.SPOT_CLIENT_URI);
        var spotifySecret = await _secretClient.AccessSecretVersionAsync(SecretStrings.SPOT_SECRET_URI);

        var spotifyClientStr = spotifyClient.Payload.Data.ToString() ?? throw new ArgumentException("No Spotify Client ID returned");
        var spotifySecretStr = spotifySecret.Payload.Data.ToString() ?? throw new ArgumentException("No Spotify Secret ID returned");

        var refreshed = await new OAuthClient()
            .RequestToken(new AuthorizationCodeRefreshRequest(spotifyClientStr, spotifySecretStr, user.refresh_token));

        var authenticator = new AuthorizationCodeAuthenticator(spotifyClientStr, spotifySecretStr, new()
        {
            AccessToken = refreshed.AccessToken,
            TokenType = refreshed.TokenType,
            ExpiresIn = refreshed.ExpiresIn,
            Scope = refreshed.Scope,
            RefreshToken = refreshed.RefreshToken ?? user.refresh_token,
            CreatedAt = refreshed.CreatedAt
        });

        authenticator.TokenRefreshed += (sender, resp) =>
        {
            
        };

        var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(authenticator);

        return config;
    }
}