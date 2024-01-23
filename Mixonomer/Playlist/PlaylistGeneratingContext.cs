using SpotifyAPI.Web;

namespace Mixonomer.Playlist;

public class PlaylistGeneratingContext
{
    public IList<PlaylistTrack<IPlayableItem>> PartTracks { get; set; }
    public IList<SavedTrack> LibraryTracks { get; set; }
}