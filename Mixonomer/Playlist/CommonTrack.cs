using SpotifyAPI.Web;

namespace Mixonomer.Playlist;

public class CommonTrack
{
    public string TrackUri { get; set; }
    public string TrackName { get; set; }
    public string AlbumName { get; set; }
    public IEnumerable<string> ArtistNames { get; set; }
    public IEnumerable<string> AlbumArtistNames { get; set; }
    public int DiscNumber { get; set; }
    public int TrackNumber { get; set; }
    public DateTime? AddedTime { get; set; }
    public string ReleaseDate { get; set; }

    public static explicit operator CommonTrack(PlaylistTrack<IPlayableItem> track)
    {
        if (track.Track is FullTrack fullTrack)
        {
            return new()
            {
                TrackUri = fullTrack.Uri,
                TrackName = fullTrack.Name,
                AlbumName = fullTrack.Album.Name,
                ArtistNames = fullTrack.Artists.Select(x => x.Name).ToArray(),
                AlbumArtistNames = fullTrack.Album.Artists.Select(x => x.Name).ToArray(),
                AddedTime = track.AddedAt,
                ReleaseDate = fullTrack.Album.ReleaseDate,
                DiscNumber = fullTrack.DiscNumber,
                TrackNumber = fullTrack.TrackNumber
            };
        }
        else
        {
            throw new InvalidCastException($"Nested track of type {track.Track.GetType()}");
        }
    }

    public static implicit operator CommonTrack(SavedTrack track)
    {
        return new()
        {
            TrackUri = track.Track.Uri,
            TrackName = track.Track.Name,
            AlbumName = track.Track.Album.Name,
            ArtistNames = track.Track.Artists.Select(x => x.Name).ToArray(),
            AlbumArtistNames = track.Track.Album.Artists.Select(x => x.Name).ToArray(),
            AddedTime = track.AddedAt,
            ReleaseDate = track.Track.Album.ReleaseDate,
            DiscNumber = track.Track.DiscNumber,
            TrackNumber = track.Track.TrackNumber
        };
    }

    public static implicit operator CommonTrack(SimpleTrack track)
    {
        return new()
        {
            TrackUri = track.Uri,
            TrackName = track.Name,
            AlbumName = null,
            ArtistNames = track.Artists.Select(x => x.Name).ToArray(),
            AlbumArtistNames = Enumerable.Empty<string>(),
            AddedTime = null,
            ReleaseDate = null,
            DiscNumber = track.DiscNumber,
            TrackNumber = track.TrackNumber
        };
    }
}