using System;
using System.Linq;
using Mixonomer.Fire;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mixonomer.Fire.Extensions;
using Mixonomer;
using Mixonomer.Playlist;

namespace Mixonomer.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var repo = new UserRepo(projectId: System.Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT"));

        var userContext = await repo.GetUserContext("andy");

        Console.WriteLine(userContext.User);

        var walker = new PartTreeWalker(repo);
        var partPlaylists = await walker.GetPlaylistParts("andy", "RAP");

        var spotifyNetwork = new SpotifyNetworkProvider(repo, null, NullLogger<SpotifyNetworkProvider>.Instance);

        var generator = new PlaylistGenerator(repo, spotifyNetwork, walker, NullLogger<PlaylistGenerator>.Instance);

        await generator.GeneratePlaylist("RAP", "andy");
    }
}
