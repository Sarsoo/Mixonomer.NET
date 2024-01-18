using Google.Cloud.Firestore;
using Mixonomer.Fire;
using Mixonomer.Fire.Extensions;

namespace Mixonomer.Playlist;

public class PartTreeWalker
{
    private readonly UserRepo _userRepo;

    private readonly HashSet<string> _processedPlaylists = new();
    public HashSet<string>? SpotifyPlaylistNames { get; private set; }

    public PartTreeWalker(UserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<string>> GetPlaylistParts(string username, string playlistName)
    {
        var user = await _userRepo.GetUser(username);

        return await GetPlaylistParts(user, playlistName);
    }

    public async Task<IEnumerable<string>> GetPlaylistParts(User user, string playlistName)
    {
        var playlist = await _userRepo.GetPlaylists(user).Where(x => x.name == playlistName).FirstOrDefaultAsync();

        if (playlist is not null)
        {
            SpotifyPlaylistNames = new HashSet<string>(playlist.parts);

            foreach (var part in playlist.playlist_references)
            {
                await ProcessPlaylist(part);
            }
        }

        return SpotifyPlaylistNames;
    }

    private async Task ProcessPlaylist(DocumentReference documentReference)
    {
        if (!_processedPlaylists.Contains(documentReference.Id))
        {
            var playlist = (await documentReference.GetSnapshotAsync()).ConvertTo<Fire.Playlist>();

            _processedPlaylists.Add(documentReference.Id);
            foreach (var p in playlist.parts)
            {
                SpotifyPlaylistNames?.Add(p);
            }

            foreach (var p in playlist.playlist_references)
            {
                await ProcessPlaylist(p);
            }
        }
    }
}