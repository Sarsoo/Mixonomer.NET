using System.Collections;
using SpotifyAPI.Web;

namespace Mixonomer.Playlist;

public class PlaylistGeneratingContext
{
    public IList<PlaylistTrack<IPlayableItem>> PartTracks { get; set; }
    public IList<SavedTrack> LibraryTracks { get; set; }

    public IEnumerable<CommonTrack> ToCommonTracks() => PartTracks.Select(x => (CommonTrack)x)
        .Concat(LibraryTracks.Select(x => (CommonTrack)x));
}