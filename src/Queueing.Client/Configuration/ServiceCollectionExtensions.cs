using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Lib.Queueing.Client.MessageControl;

namespace Lib.Queueing.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IQueueClientBuilder AddQueueClientBuilder(this IServiceCollection services)
        {
            return new QueueClientBuilder(services);
        }

        public static IQueueClientBuilder AddQueueClient(this IServiceCollection services, Action<QueueClientOption> options)
        {
            var builder = services.AddQueueClientBuilder();
            services
                .AddOptions<QueueClientOption>()
                .Configure(options)
                .ValidateDataAnnotations();

            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<QueueClientOption>>().Value);
            builder.Services.AddTransient<IQueueClient, QueueClient>();
            builder.Services.AddTransient<IMessageControlService, MessageControlService>();

            return builder;

        }



        

    }
}