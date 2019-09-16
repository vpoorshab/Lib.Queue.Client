using Microsoft.Extensions.DependencyInjection;

namespace Lib.Queueing.Client
{
    public interface ISubscriptionBuilder
    {
        IServiceCollection Services { get; }


    }
}