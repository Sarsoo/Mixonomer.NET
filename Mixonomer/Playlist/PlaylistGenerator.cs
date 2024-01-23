using Microsoft.Extensions.Logging;
using Mixonomer.Exceptions;
using Mixonomer.Fire;
using Mixonomer.Fire.Extensions;
using Mixonomer.Playlist.Sort;
using SpotifyAPI.Web;

namespace Mixonomer.Playlist;

public class PlaylistGenerator
{
    private readonly ILogger<PlaylistGenerator> _logger;
    protected readonly UserRepo _userRepo;
    private readonly SpotifyNetworkProvider _spotifyMetworkProvider;
    protected readonly PartTreeWalker _partTreeWalker;

    public PlaylistGenerator(UserRepo userRepo, SpotifyNetworkProvider spotifyMetworkProvider, PartTreeWalker partTreeWalker, ILogger<PlaylistGenerator> logger)
    {
        _userRepo = userRepo;
        _spotifyMetworkProvider = spotifyMetworkProvider;
        _logger = logger;
        _partTreeWalker = partTreeWalker;
    }

    public async Task GeneratePlaylist(string playlistName, string username)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, string> { {"username", username}, {"playlist", playlistName} });

        var user = await _userRepo.GetUser(username);
        var dbPlaylist = await _userRepo.GetPlaylists(user).FirstOrDefaultAsync(x => x.name == playlistName);

        if (dbPlaylist is null)
        {
            _logger.LogError("Couldn't find playlist in database");
            throw new PlaylistNotFoundException();
        }

        var spotifyClient = new SpotifyClient(await _spotifyMetworkProvider.GetUserConfig(user));

        var userPlaylists = await spotifyClient.Playlists.CurrentUsers();
        var allPlaylists = await spotifyClient.PaginateAll(userPlaylists);

        var parts = await GetFullPartList(user, dbPlaylist);
        var partPlaylists = GetPartPlaylists(dbPlaylist, allPlaylists, parts);

        var context = new PlaylistGeneratingContext
        {
            PartTracks = await GetPlatlistTracks(spotifyClient, partPlaylists).ToListAsync(),
            LibraryTracks = await GetLibraryTracks(spotifyClient, dbPlaylist).ToListAsync()
        };

        context = DoPlaylistTypeProcessing(context, user, dbPlaylist);

        var combinedTracks = CollapseContextToCommonTracks(context);
        combinedTracks = SortTracks(combinedTracks, dbPlaylist);
    }

    private async Task<IEnumerable<string>> GetFullPartList(User user, Fire.Playlist playlist)
    {
        var parts = (await _partTreeWalker.GetPlaylistParts(user, playlist.name) ?? Enumerable.Empty<string>()).ToList();

        if (playlist.add_last_month)
        {
            parts.Add(Months.LastMonth());
        }

        if (playlist.add_this_month)
        {
            parts.Add(Months.ThisMonth());
        }

        if (parts.Count == 0)
        {
            _logger.LogInformation("No spotify playlist parts found");
        }

        return parts;
    }

    private IEnumerable<FullPlaylist> GetPartPlaylists(Fire.Playlist subjectPlaylist, IEnumerable<FullPlaylist> allPlaylists, IEnumerable<string> parts)
    {
        var allPlaylistDict = allPlaylists.ToDictionary(p => p.Name ?? "no name");

        foreach (var part in parts)
        {
            if (allPlaylistDict.TryGetValue(part, out var playlist))
            {
                if (!subjectPlaylist.include_spotify_owned &&
                    (playlist.Owner?.DisplayName.Contains("spotify", StringComparison.InvariantCultureIgnoreCase) ?? false))
                {
                    // skip
                }
                else
                {
                    yield return playlist;
                }
            }
        }
    }

    private async IAsyncEnumerable<PlaylistTrack<IPlayableItem>> GetPlatlistTracks(SpotifyClient client, IEnumerable<FullPlaylist> playlists)
    {
        foreach (var playlist in playlists)
        {
            if (playlist.Tracks is { } tracks)
            {
                foreach (var track in await client.PaginateAll(tracks))
                {
                    yield return track;
                }
            }
        }
    }

    private async IAsyncEnumerable<SavedTrack> GetLibraryTracks(SpotifyClient client, Fire.Playlist playlist)
    {
        if (playlist.include_library_tracks)
        {
            await foreach(var track in client.Paginate(await client.Library.GetTracks()))
            {
                yield return track;
            }
        }
    }

    protected virtual PlaylistGeneratingContext DoPlaylistTypeProcessing(PlaylistGeneratingContext context, User user, Fire.Playlist playlist)
    {
        return context;
    }

    protected virtual IEnumerable<CommonTrack> CollapseContextToCommonTracks(PlaylistGeneratingContext context)
    {
        return context.PartTracks.Select(x => (CommonTrack)x)
            .Concat(context.LibraryTracks.Select(x => (CommonTrack)x));
    }

    protected virtual IEnumerable<CommonTrack> SortTracks(IEnumerable<CommonTrack> tracks, Fire.Playlist playlist)
    {
        if (playlist.shuffle)
        {
            return tracks.Shuffle();
        }
        else
        {
            return tracks.OrderByReleaseDate();
        }
    }
}