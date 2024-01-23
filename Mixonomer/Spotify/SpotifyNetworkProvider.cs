using Google.Cloud.Firestore;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Logging;
using Mixonomer.Fire;
using SpotifyAPI.Web;

namespace Mixonomer;

public class SpotifyNetworkProvider
{
    private readonly SecretManagerServiceClient _secretClient;
    private readonly UserRepo _userRepo;
    private readonly ILogger<SpotifyNetworkProvider> _logger;

    public SpotifyNetworkProvider(UserRepo userRepo, SecretManagerServiceClient? secretClient, ILogger<SpotifyNetworkProvider> logger)
    {
        _userRepo = userRepo;
        _logger = logger;
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

        var spotifyClientStr = spotifyClient.Payload.Data.ToStringUtf8() ?? throw new ArgumentException("No Spotify Client ID returned");
        var spotifySecretStr = spotifySecret.Payload.Data.ToStringUtf8() ?? throw new ArgumentException("No Spotify Secret returned");

        var refreshed = await new OAuthClient()
            .RequestToken(new AuthorizationCodeRefreshRequest(spotifyClientStr, spotifySecretStr, user.refresh_token));

        await WriteUserTokenUpdate(user, new
        {
            access_token = refreshed.AccessToken,
            refresh_token = refreshed.RefreshToken,
            last_refreshed = refreshed.CreatedAt,
            token_expiry = refreshed.ExpiresIn
        });

        var authenticator = new AuthorizationCodeAuthenticator(spotifyClientStr, spotifySecretStr, new()
        {
            AccessToken = refreshed.AccessToken,
            TokenType = refreshed.TokenType,
            ExpiresIn = refreshed.ExpiresIn,
            Scope = refreshed.Scope,
            RefreshToken = refreshed.RefreshToken ?? user.refresh_token,
            CreatedAt = refreshed.CreatedAt
        });

        authenticator.TokenRefreshed += async (sender, resp) =>
        {
            await WriteUserTokenUpdate(user, new
            {
                access_token = resp.AccessToken,
                refresh_token = resp.RefreshToken,
                last_refreshed = resp.CreatedAt,
                token_expiry = resp.ExpiresIn
            });
        };

        var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(authenticator);

        return config;
    }

    private async Task WriteUserTokenUpdate(User user, object updates)
    {
        try
        {
            _logger.LogInformation("Token refreshed for [{}], writing to database", user.username);
            await user.Reference.SetAsync(updates, SetOptions.MergeAll);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to write updated Spotify tokens to database for [{}]", user.username);
        }
    }
}