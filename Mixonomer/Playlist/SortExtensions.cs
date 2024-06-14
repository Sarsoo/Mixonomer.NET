namespace Mixonomer.Playlist.Sort;

public static class SortExtensions
{
    private static Random _rng = new Random();

    public static IOrderedEnumerable<CommonTrack> OrderByArtistAlbumTrackNumber(this IEnumerable<CommonTrack> input) =>
        input.OrderBy(x => x.AlbumArtistNames.FirstOrDefault())
            .ThenBy(x => x.AlbumName)
            .ThenBy(x => x.DiscNumber)
            .ThenBy(x => x.TrackNumber);

    public static IOrderedEnumerable<CommonTrack> OrderByReleaseDate(this IEnumerable<CommonTrack> input) =>
        input.OrderByDescending(x => x.ReleaseDate)
            .ThenBy(x => x.AlbumArtistNames.FirstOrDefault())
            .ThenBy(x => x.AlbumName)
            .ThenBy(x => x.DiscNumber)
            .ThenBy(x => x.TrackNumber);

    public static IOrderedEnumerable<CommonTrack> Shuffle(this IEnumerable<CommonTrack> input) =>
        input.OrderBy(x => _rng.Next());
}