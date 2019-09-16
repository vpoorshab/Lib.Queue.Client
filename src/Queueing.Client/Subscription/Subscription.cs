using System;

namespace Lib.Queueing.Client
{
    public class Subscription : DefaultMessageQueueSetting , ISubscription
    {
       
       
        public Subscription(string systemIdentifier, string eventType, string exchange, Type processorType) : base(systemIdentifier,eventType,exchange)
        {
           
            ProcessorType = processorType;
            Identifier = Guid.NewGuid();
        }


        public Guid Identifier { get; }
        public bool IsRunning { get; set; }
        public Type ProcessorType { get; }

    }
}