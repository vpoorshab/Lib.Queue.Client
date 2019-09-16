using System.Collections.Generic;

namespace Lib.Queueing.Client
{
    public class DefaultMessageQueueSetting : IMessageQueueSetting
    {
       
        public DefaultMessageQueueSetting(string systemIdentifier, string eventType, string exchange)
        {
            SystemIdentifier = systemIdentifier;
            EventType = eventType;
            Exchange = exchange;
        }

        public DefaultMessageQueueSetting(string eventType, string exchange) : this("", eventType, exchange) { }


        public string SystemIdentifier { get; }

        public string EventType { get; }

        public string Exchange { get; }

        /// <summary>
        /// return Queue Name based on convention : {SystemIdentifier}.{EventType}
        /// </summary>
        public string Queue => !string.IsNullOrWhiteSpace(SystemIdentifier) && !string.IsNullOrWhiteSpace(EventType) ? $"{SystemIdentifier}.{EventType}" : string.Empty;

    }
}