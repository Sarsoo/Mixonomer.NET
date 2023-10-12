using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Mixonomer.Fire;

namespace Mixonomer.Func
{
    public class RunUserPlaylist : ICloudEventFunction<MessagePublishedData>
    {
        private readonly ILogger _logger;

        public RunUserPlaylist(ILogger<RunUserPlaylist> logger)
        {
            _logger = logger;
        }


        public async Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received message in C# {data.Message}, {cloudEvent.GetPopulatedAttributes()}");

            var userRepo = new UserRepo(projectId: System.Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT"));

            var user = await userRepo.GetUser(data.Message.Attributes["username"]);

            _logger.LogInformation($"{user.username} was last refreshed at {user.last_refreshed}");
        }
    }
}

