namespace Mixonomer.Playlist;

public static class SecretStrings
{
    public static Lazy<string> GoogleProject = new(() => Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") ??
                                                         throw new ArgumentException("no GOOGLE_CLOUD_PROJECT env variable available"));

    public static string SPOT_CLIENT_URI => $"projects/{GoogleProject.Value}/secrets/spotify-client/versions/latest";
    public static string SPOT_SECRET_URI => $"projects/{GoogleProject.Value}/secrets/spotify-secret/versions/latest";
    public static string LASTFM_CLIENT_URI => $"projects/{GoogleProject.Value}/secrets/lastfm-client/versions/latest";
    public static string JWT_SECRET_URI => $"projects/{GoogleProject.Value}/secrets/jwt-secret/versions/latest";
    public static string COOKIE_SECRET_URI => $"projects/{GoogleProject.Value}/secrets/cookie-secret/versions/latest";
    public static string APNS_SIGN_URI => $"projects/{GoogleProject.Value}/secrets/apns-auth-sign-key/versions/1";

    public static string STATIC_BUCKET => $"{GoogleProject.Value}-static";
}