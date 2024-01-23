namespace Mixonomer.Playlist;

public interface IRecommend
{
    Task<IEnumerable<CommonTrack>> GetRecommendations(Fire.Playlist playlist, IEnumerable<CommonTrack> currentTrackList);
}