using System;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lib.Queueing.Client.MessageControl
{
    public class MessageControlService : IMessageControlService
    {
        private readonly ILogger<MessageControlService> _logger;
        private readonly IQueueClient _queueClient;
        private readonly QueueClientOption _queueClientOption;
       
        public MessageControlService(ILogger<MessageControlService> logger, IQueueClient queueClient, QueueClientOption queueClientOption)
        {
            _logger = logger;
            _queueClient = queueClient;
            _queueClientOption = queueClientOption;
            
        }




        public void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting)
        {
            SendMessageControl(state, sourceMessage, messageQueueSetting, null, null);
        }

        public void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, StateResponse stateResponse)
        {
            SendMessageControl(state,sourceMessage, messageQueueSetting, null,stateResponse);
        }

        public void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, Exception exception)
        {
            SendMessageControl(state, sourceMessage, messageQueueSetting, exception, null);
        }

        public void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, Exception exception, StateResponse stateResponse)
        {
            if (_queueClientOption.DisableMessageControl) return;

            try
            {
                using (var connection = _queueClient.Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(_queueClientOption.MessageControlExchange, "topic", true);

                    var messageControl = new MessageControl()
                    {
                        MessageId = sourceMessage.MessageID,
                        CorrelationId = sourceMessage.CorrelationID,
                        MessageType = sourceMessage.Type,
                        State = state,
                        Exchange = messageQueueSetting.Exchange,
                        Queue = messageQueueSetting.Queue,
                        RoutingKey = new []{messageQueueSetting.EventType},
                    };


                    var message = new Message(){CreatedSource = _queueClientOption.SystemIdentifier};

                    message.SetPayload(messageControl);

                    if (sourceMessage.Keys != null && sourceMessage.Keys.Count > 0)
                    {
                        foreach (var (key, value) in sourceMessage.Keys)
                        {
                            message.Keys.Add(key, value);
                        }
                    }

                    if (stateResponse != null)
                    {
                        messageControl.StateResponse = stateResponse;
                    }


                    if (exception != null)
                    {
                        messageControl.StateResponse.IsSuccess = false;
                        messageControl.StateResponse.IsRecoverable = (state == MessageState.Retry);
                        messageControl.StateResponse.StateData.Add(StateData.GetExceptionStateData(exception));
                    }


                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    var body = Encoding.UTF8.GetBytes(message.ToString());
                    channel.BasicPublish(_queueClientOption.MessageControlExchange, "", properties, body);
                }


            }
            catch (Exception ex2)
            {

                _logger.LogError(ex2, "Unable to write message messageControl.");
            }

        }
    }
}
