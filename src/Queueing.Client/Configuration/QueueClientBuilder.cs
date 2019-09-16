using System;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Queueing.Client
{
    public class QueueClientBuilder : IQueueClientBuilder
    {
        public IServiceCollection Services { get; }

        public QueueClientBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}