using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;

namespace Mixonomer.Func
{
    public class RunUserPlaylist : ICloudEventFunction<MessagePublishedData>
    {
        private readonly ILogger _logger;

        public RunUserPlaylist(ILogger<RunUserPlaylist> logger)
        {
            _logger = logger;
        }


        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Received message in C# {data.Message}, {cloudEvent.GetPopulatedAttributes()}");

            return Task.CompletedTask;
        }
    }
}

