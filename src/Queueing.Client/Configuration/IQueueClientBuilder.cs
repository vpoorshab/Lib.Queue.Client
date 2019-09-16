using Microsoft.Extensions.DependencyInjection;

namespace Lib.Queueing.Client
{
    public interface IQueueClientBuilder
    {
        IServiceCollection Services { get; }


    }


}