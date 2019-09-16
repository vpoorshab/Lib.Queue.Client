using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lib.Queueing.Client.MessageControl
{
    public class MessageControl : IStateResponse
    {

       

        /// <summary>
        /// The identifier of the message being controlled.
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// The correlation identifier for the root message for any retry messages.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// The type of the message being controlled.
        /// </summary>
        public string MessageType;

        /// <summary>
        /// The current state of the message control life cycle.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageState State;

        /// <summary>
        /// The exchange the message is being broadcast to.
        /// </summary>
        public string Exchange;

        /// <summary>
        /// The queue the message is being subscribed from.
        /// </summary>
        public string Queue;

        /// <summary>
        /// The queue the message is being subscribed from.
        /// </summary>
        public string[] RoutingKey;


        public StateResponse StateResponse
        {
            get;
            set;
        } = new StateResponse();


    }
}
