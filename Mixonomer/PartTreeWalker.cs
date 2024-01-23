using Google.Cloud.Firestore;
using Mixonomer.Fire;
using Mixonomer.Fire.Extensions;

namespace Mixonomer;

public class PartTreeWalker
{
    private readonly UserRepo _userRepo;

    private readonly HashSet<string> _processedPlaylists = new();
    public HashSet<string>? SpotifyPlaylistNames { get; private set; }
    private List<Fire.Playlist> _userPlaylists;

    public PartTreeWalker(UserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<string>?> GetPlaylistParts(string username, string playlistName)
    {
        var user = await _userRepo.GetUser(username);

        return await GetPlaylistParts(user, playlistName);
    }

    public async Task<IEnumerable<string>?> GetPlaylistParts(User user, string playlistName)
    {
        _userPlaylists = await _userRepo.GetPlaylists(user).ToListAsync();
        var playlist = _userPlaylists.SingleOrDefault(x => x.name == playlistName);

        if (playlist is not null)
        {
            SpotifyPlaylistNames = new HashSet<string>(playlist.parts);

            foreach (var part in playlist.playlist_references)
            {
                ProcessPlaylist(part);
            }
        }

        return SpotifyPlaylistNames;
    }

    private void ProcessPlaylist(DocumentReference documentReference)
    {
        if (!_processedPlaylists.Contains(documentReference.Id))
        {
            var playlist = _userPlaylists.SingleOrDefault(x => x.Reference.Id == documentReference.Id);

            _processedPlaylists.Add(documentReference.Id);
            if (playlist != null)
            {
                foreach (var p in playlist.parts)
                {
                    SpotifyPlaylistNames?.Add(p);
                }

                foreach (var p in playlist.playlist_references)
                {
                    ProcessPlaylist(p);
                }
            }
        }
    }
}