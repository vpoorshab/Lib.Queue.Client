using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lib.Queueing.Client.HostingService
{
    public class SubscriptionHostedService : IHostedService
    {
        private readonly ILogger<SubscriptionHostedService> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEnumerable<ISubscription> _subscriptions;
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public SubscriptionHostedService(ILogger<SubscriptionHostedService> logger, ISubscriptionService subscriptionService, IEnumerable<Subscription> subscriptions)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _subscriptions = subscriptions;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Queue Rider is starting....");


            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);


            // Store the task we're executing
            _executingTask = ExecuteAsync(_cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }


        protected Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queue Rider is Executing....");

            cancellationToken.Register(() => _logger.LogInformation("Queue Rider Cancellation Token is requested ...."));

            cancellationToken.ThrowIfCancellationRequested();

            List<Task> subscriptionsTasks = new List<Task>();

            try
            {

                foreach (var subscription in _subscriptions)
                {

                    subscriptionsTasks.Add(_subscriptionService.StartSubscriptionAsync(subscription, cancellationToken));
                }

               
                return Task.WhenAll(subscriptionsTasks);


            }
            catch (Exception e)
            {

                _logger.LogError(e, "exception on Queue Rider");
            }
            //}

            return Task.CompletedTask;
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queue Rider is stopping....");

            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
