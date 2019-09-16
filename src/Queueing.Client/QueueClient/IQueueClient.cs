using RabbitMQ.Client;

namespace Lib.Queueing.Client
{
    public interface IQueueClient
    {
        QueueClientOption Options { get; }
        ConnectionFactory Factory { get; }
    }
}