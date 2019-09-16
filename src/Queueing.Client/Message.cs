using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lib.Queueing.Client
{
    public class Message : IMessage
    {
        public Message()
        {
            MessageID = Guid.NewGuid();
            Created = DateTime.UtcNow;
            CorrelationID = null;
        }

       
        public Guid MessageID { get; set; }

        public Guid? CorrelationID { get; set; }

        public object Payload { get; set; }

        public string Type { get; set; }

        public DateTime Created { get; set; }

        public string CreatedSource { get; set; }

        public string CreatedBy { get; set; }

        public string Exchange { get; set; }

        public string[] RoutingKey { get; set; }

        public Dictionary<string, string> Keys 
        {
            get;
        } = new Dictionary<string, string>();


        public void SetPayload<T>(T payload)
        {
            Type = typeof(T).FullName;

            Payload = payload;

        }

        public T GetPayloadAs<T>()
        {
            try
            {
                if (Payload != null)
                {

                    return ((JObject)Payload).ToObject<T>();
                    
                }

                return default(T);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string ToString()
        {

           return JsonConvert.SerializeObject(this);
        }

        
    }


    
}