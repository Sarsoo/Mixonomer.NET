using Microsoft.Extensions.Logging;
using Mixonomer.Fire;
using SpotifyAPI.Web;

namespace Mixonomer;

public class PlaylistGenerator
{
    private readonly ILogger<PlaylistGenerator> _logger;
    private readonly UserRepo _userRepo;
    private readonly SpotifyNetworkProvider _spotifyMetworkProvider;
    private readonly PartTreeWalker _partTreeWalker;

    public PlaylistGenerator(UserRepo userRepo, SpotifyNetworkProvider spotifyMetworkProvider, PartTreeWalker partTreeWalker, ILogger<PlaylistGenerator> logger)
    {
        _userRepo = userRepo;
        _spotifyMetworkProvider = spotifyMetworkProvider;
        _logger = logger;
        _partTreeWalker = partTreeWalker;
    }

    public async Task GeneratePlaylist(string playlistName, string username)
    {
        var user = await _userRepo.GetUser(username);

        var spotifyConfig = await _spotifyMetworkProvider.GetUserConfig(user);
        var spotifyClient = new SpotifyClient(spotifyConfig);

        var userPlaylists = await spotifyClient.Playlists.CurrentUsers();
        var allPlaylists = await spotifyClient.PaginateAll(userPlaylists);
    }
}