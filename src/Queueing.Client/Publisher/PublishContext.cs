using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lib.Queueing.Client
{
    public class PublishContext
    {
        public string EventType { get; set; }
        public string Exchange { get; set; }
        public Dictionary<string, string> Keys { get; set; } = new Dictionary<string, string>();
        public string CreatedSource { get; set; }
        public string CreatedBy { get; set; }
        public Guid? CorrelationId { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}