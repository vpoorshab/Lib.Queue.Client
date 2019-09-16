using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Lib.Queueing.Client
{
    public interface ISubscriptionService
    {
        Task StartSubscriptionAsync(ISubscription subscription, CancellationToken cancellationToken);


    }
}
