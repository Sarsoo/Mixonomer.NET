using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Mixonomer.Exceptions;
using Mixonomer.Extensions;
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

        ArgumentException.ThrowIfNullOrWhiteSpace(username);

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
            PartTracks = await GetPlaylistTracks(spotifyClient, partPlaylists).ToListAsync(),
            LibraryTracks = await GetLibraryTracks(spotifyClient, dbPlaylist).ToListAsync()
        };

        context = DoPlaylistTypeProcessing(context, user, dbPlaylist);

        var combinedTracks = CollapseContextToCommonTracks(context);

        // var recommender = new SpotifyRecommender(spotifyClient);
        // var recommendations = await recommender.GetRecommendations(dbPlaylist, combinedTracks);
        //
        // combinedTracks = combinedTracks.Concat(recommendations);

        // combinedTracks = combinedTracks.DistinctBy(x => (x.TrackName, string.Join(':', x.ArtistNames.Order())));
        // combinedTracks = combinedTracks.DistinctBy(x => x.TrackUri);
        combinedTracks = combinedTracks.DistinctBy(x => (x.TrackName.ToLower(), string.Concat(x.ArtistNames.Order())));

        combinedTracks = SortTracks(combinedTracks, dbPlaylist);

        await ExecutePlaylist(spotifyClient, dbPlaylist, user, combinedTracks, parts);

        await dbPlaylist.Reference.SetAsync(new
        {
            last_updated = DateTime.UtcNow
        }, SetOptions.MergeAll);
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
        foreach (var playlist in allPlaylists)
        {
            foreach (var part in parts)
            {
                if (playlist.Name?.Equals(part, StringComparison.Ordinal) ?? false)
                {
                    if (!subjectPlaylist.include_spotify_owned &&
                        (playlist.Owner?.DisplayName.Contains("spotify", StringComparison.InvariantCultureIgnoreCase) ??
                         false))
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
    }

    private async IAsyncEnumerable<PlaylistTrack<IPlayableItem>> GetPlaylistTracks(SpotifyClient client, IEnumerable<FullPlaylist> playlists)
    {
        foreach (var playlist in playlists)
        {
            if (playlist.Tracks is { } tracks)
            {
                foreach (var track in await client.PaginateAll(await client.Playlists.GetItems(playlist.Id)))
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

    protected virtual IEnumerable<CommonTrack> CollapseContextToCommonTracks(PlaylistGeneratingContext context) =>
        context.ToCommonTracks();

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

    protected virtual async Task ExecutePlaylist(SpotifyClient client, Fire.Playlist playlist, User user, IEnumerable<CommonTrack> tracks, IEnumerable<string> partList)
    {
        var chunks = tracks.Select(x => x.TrackUri).Chunk(100);
        var playlistId = playlist.uri.UriToId();

        if (chunks.FirstOrDefault() is { } chunk)
        {
            await client.Playlists.ReplaceItems(playlist.uri.UriToId(),
                new PlaylistReplaceItemsRequest(chunk.ToList()));
        }

        foreach (var remainingChunk in chunks.Skip(1))
        {
            await client.Playlists.AddItems(playlistId, new PlaylistAddItemsRequest(remainingChunk));
        }
    }
}