using System;
using System.Collections.Generic;

namespace Lib.Queueing.Client
{

    public interface IMessage
    {
        Guid MessageID { get; set; }
        Guid? CorrelationID { get; set; }
        string Type { get; }
        Dictionary<string, string> Keys { get; }
        object Payload { get; }
        DateTime Created { get; set; }
        string CreatedSource { get; set; }
        string CreatedBy { get; set; }
        string Exchange { get; set; }
        //Keep RoutingKey because of backward compatibility 
        string[] RoutingKey { get; set; }

        void SetPayload<T>(T payload);
        T GetPayloadAs<T>();
    }

    
}