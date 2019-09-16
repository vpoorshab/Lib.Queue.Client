using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Lib.Queueing.Client.MessageControl;
using RabbitMQ.Client;

namespace Lib.Queueing.Client
{
    public class Publisher : IPublisher
    {
        private readonly ILogger<Publisher> _logger;
        private readonly IQueueClient _queueClient;
        private readonly IMessageControlService _messageControlService;

        public Publisher(ILogger<Publisher> logger, IQueueClient queueClient, IMessageControlService messageControlService)
        {

            _logger = logger;
            _queueClient = queueClient;
            _messageControlService = messageControlService;
        }

        public Guid PublishMessage<T>(T payload, PublishContext publishContext)
        {




            try
            {
                if (string.IsNullOrWhiteSpace(publishContext.CreatedSource))
                {
                    throw new ArgumentNullException(nameof(publishContext.CreatedSource));
                }


                var message = new Message()
                {
                    CorrelationID = publishContext.CorrelationId,
                    CreatedSource = publishContext.CreatedSource,
                    CreatedBy = publishContext.CreatedBy,
                    Exchange = publishContext.Exchange,
                    RoutingKey = new[] { publishContext.EventType }
                };


                message.SetPayload(payload);

                var messageQueueSetting = new DefaultMessageQueueSetting(_queueClient.Options.SystemIdentifier, publishContext.EventType, publishContext.Exchange);

                _messageControlService.SendMessageControl(MessageState.Sent, message, messageQueueSetting);

                Publish(message, messageQueueSetting);
                return message.MessageID;
            }
            catch (Exception ex)
            {
                _logger.LogError("log exception with scope", ex);
                throw;
            }


        }


        private void Publish(Message message, IMessageQueueSetting messageQueueSetting)
        {
            using (var connection = _queueClient.Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                if (!string.IsNullOrWhiteSpace(messageQueueSetting.Exchange))
                {
                    channel.ExchangeDeclare(messageQueueSetting.Exchange, "topic", true);

                    if (!_queueClient.Options.DisableDeadLettering)
                    {
                        channel.ExchangeDeclare(messageQueueSetting.Exchange + _queueClient.Options.DeadLetterPostFix, "topic", true);
                    }
                }

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                string payload = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(payload);
                channel.BasicPublish(messageQueueSetting.Exchange, messageQueueSetting.EventType, properties, body);
            }

        }
    }
}
