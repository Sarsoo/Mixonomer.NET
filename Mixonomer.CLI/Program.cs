using System;
using Mixonomer.Fire;
using System.Threading.Tasks;

namespace Mixonomer.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repo = new UserRepo(projectId: "sarsooxyz");

            var user = await repo.GetUser("andy");
            var playlists = await repo.GetPlaylists("andy");

            await foreach (var playlist in playlists)
            {
                var dict = playlist.ToDictionary();
                var playlistObj = playlist.ConvertTo<Playlist>();
            }

            Console.WriteLine(user);
        }
    }
}

