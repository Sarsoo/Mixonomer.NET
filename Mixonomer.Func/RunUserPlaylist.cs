using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mixonomer.Fire;
using Mixonomer.Playlist;
using SpotifyAPI.Web;

namespace Mixonomer.Func;

[FunctionsStartup(typeof(RunUserPlaylistStartup))]
public class RunUserPlaylist : ICloudEventFunction<MessagePublishedData>
{
    private readonly ILogger _logger;
    private readonly UserRepo _userRepo;
    private readonly SpotifyNetworkProvider _spotifyMetworkProvider;

    public RunUserPlaylist(ILogger<RunUserPlaylist> logger, UserRepo userRepo, SpotifyNetworkProvider spotifyMetworkProvider)
    {
        _logger = logger;
        _userRepo = userRepo;
        _spotifyMetworkProvider = spotifyMetworkProvider;
    }


    public async Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received message in C# {data.Message}, {cloudEvent.GetPopulatedAttributes()}");

        var user = await _userRepo.GetUser(data.Message.Attributes["username"]);

        var spotifyConfig = await _spotifyMetworkProvider.GetUserConfig(user);
        var spotifyClient = new SpotifyClient(spotifyConfig);

        _logger.LogInformation($"{user.username} was last refreshed at {user.last_refreshed}");
    }
}

public class RunUserPlaylistStartup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        base.ConfigureServices(context, services);

        services.AddSecretManagerServiceClient();
        // services.AddFirestoreClient();

        services.AddTransient<SpotifyNetworkProvider>()
                .AddTransient<PlaylistGenerator>()
                .AddSingleton<UserRepo>();
    }
}