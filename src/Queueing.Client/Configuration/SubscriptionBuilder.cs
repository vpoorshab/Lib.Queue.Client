using System;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Queueing.Client
{
    public class SubscriptionBuilder : ISubscriptionBuilder
    {
        public IServiceCollection Services { get; }

        public SubscriptionBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}