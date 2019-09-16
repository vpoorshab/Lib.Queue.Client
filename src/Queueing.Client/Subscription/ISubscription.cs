using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lib.Queueing.Client
{
    public interface ISubscription : IMessageQueueSetting
    {
        Guid Identifier { get; }

        bool IsRunning { get; set; }
        
        Type ProcessorType { get; }

       
    }
}