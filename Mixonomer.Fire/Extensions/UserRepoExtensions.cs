using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Mixonomer.Fire.Model;

namespace Mixonomer.Fire.Extensions;

public static class UserRepoExtensions
{
    public static async IAsyncEnumerable<User> GetUsers(this UserRepo repo)
    {
        var users = repo.GetUserDocs();

        await foreach (var user in users)
        {
            yield return user.ConvertTo<User>();
        }
    }

    public static async Task<IAsyncEnumerable<DocumentSnapshot>> GetPlaylistDocs(this UserRepo repo, string username)
    {
        var user = await repo.GetUser(username).ConfigureAwait(false);

        return repo.GetPlaylistDocs(user);
    }

    public static async IAsyncEnumerable<Playlist> GetPlaylists(this UserRepo repo, User user)
    {
        var playlists = repo.GetPlaylistDocs(user);

        await foreach (var playlist in playlists)
        {
            yield return playlist.ConvertTo<Playlist>();
        }
    }

    public static async Task<IAsyncEnumerable<DocumentSnapshot>> GetTagDocs(this UserRepo repo, string username)
    {
        var user = await repo.GetUser(username).ConfigureAwait(false);

        return repo.GetTagDocs(user);
    }

    public static async IAsyncEnumerable<Tag> GetTags(this UserRepo repo, User user)
    {
        var tags = repo.GetTagDocs(user);

        await foreach (var tag in tags)
        {
            yield return tag.ConvertTo<Tag>();
        }
    }

    public static async Task<UserContext> GetUserContext(this UserRepo repo, string username)
    {
        var user = new UserContext
        {
            User = await repo.GetUser(username).ConfigureAwait(false)
        };

        var playlists = repo.GetPlaylists(user.User).ToListAsync();
        var tags = repo.GetTags(user.User).ToListAsync();

        await Task.WhenAll(playlists.AsTask(), tags.AsTask()).ConfigureAwait(false);

        user.Playlists = playlists.Result;
        user.Tags = tags.Result;

        return user;
    }
}
