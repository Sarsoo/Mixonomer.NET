using Mixonomer.Extensions;
using Mixonomer.Playlist;
using Mixonomer.Playlist.Sort;
using SpotifyAPI.Web;

namespace Mixonomer;

public class SpotifyRecommender: IRecommend
{
    private readonly SpotifyClient _client;

    public SpotifyRecommender(SpotifyClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<CommonTrack>> GetRecommendations(Fire.Playlist playlist,
        IEnumerable<CommonTrack> currentTrackList)
    {
        if (playlist.include_recommendations)
        {
            var request = new RecommendationsRequest()
            {
                Limit = playlist.recommendation_sample
            };

            foreach (var track in currentTrackList.Shuffle().Take(5))
            {
                request.SeedTracks.Add(track.TrackUri.UriToId());
            }

            var response = await _client.Browse.GetRecommendations(request);

            return response.Tracks.Select(x => (CommonTrack) x);
        }

        return [];
    }
}