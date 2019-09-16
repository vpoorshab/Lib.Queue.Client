using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Lib.Queueing.Client.HostingService;


namespace Lib.Queueing.Client
{
    public static class QueueClientBuilderExtensions
    {
        public static IQueueClientBuilder AddSubscription(this IQueueClientBuilder client, Action<ISubscriptionBuilder> configure)
        {

            client.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
            client.Services.AddHostedService<SubscriptionHostedService>();
            configure((ISubscriptionBuilder)new SubscriptionBuilder(client.Services));

            return client;
        }


        public static ISubscriptionBuilder Subscribe<TProcessor>(this ISubscriptionBuilder builder, string eventType, string exchange)
            where TProcessor : IMessageProcessor
        {
            builder.Services.TryAddTransient(typeof(TProcessor));


            builder.Services.Add(ServiceDescriptor.Singleton(provider =>
            {

                var opt = provider.GetRequiredService<QueueClientOption>();
                return new Subscription(opt.SystemIdentifier, eventType, exchange, typeof(TProcessor));

            }));



            return builder;
        }


        public static IQueueClientBuilder AddPublisher(this IQueueClientBuilder client)
        {
            client.Services.AddSingleton<IPublisher, Publisher>();
            return client;
        }
    }
}