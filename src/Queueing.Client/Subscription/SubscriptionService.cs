using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Lib.Queueing.Client.MessageControl;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lib.Queueing.Client
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IQueueClient _queueClient;

        public SubscriptionService(IServiceProvider serviceProvider, ILogger<SubscriptionService> logger, IQueueClient queueClient)
        {

            _serviceProvider = serviceProvider;
            _logger = logger;
            _queueClient = queueClient;

        }


        public Task StartSubscriptionAsync(ISubscription subscription, CancellationToken cancellationToken)
        {

            return Task.Run(async () =>
            {


                _logger.LogInformation($"Start Subscription ...[{subscription.Identifier}]");


                //Cancellation Actions
                cancellationToken.Register(() =>
                {

                    _logger.LogInformation(
                        $"Cancellation Token Requested for Subscription : [{subscription.Identifier}]");

                });

                if (subscription.IsRunning)
                {
                    _logger.LogWarning($"The Subscription [{subscription.Identifier}] is in Running mode - cannot run again");
                    return;
                }

                //subscription.IsRunning = true;

                while (!cancellationToken.IsCancellationRequested)
                {

                    try
                    {

                        using (var connection = _queueClient.Factory.CreateConnection())
                        using (var channel = connection.CreateModel())
                        {
                            // Declare the exchange, the queue and bind them together.
                            channel.ExchangeDeclare(subscription.Exchange, "topic", true);
                            QueueDeclareOk queueDeclare = null;


                            if (!_queueClient.Options.DisableDeadLettering)
                            {
                                var deadLetterExchange =
                                    subscription.Exchange + _queueClient.Options.DeadLetterPostFix;
                                var deadLetterQueue = subscription.Queue + _queueClient.Options.DeadLetterPostFix;

                                // Create the queue and set the bindings to the dead letter exchange
                                IDictionary<String, Object> args = new Dictionary<String, Object>();
                                args.Add("x-dead-letter-exchange", deadLetterExchange);
                                args.Add("x-dead-letter-routing-key", deadLetterQueue);

                                queueDeclare = channel.QueueDeclare(subscription.Queue, true, false, false, args);


                                // create the dead letter queue and bind it ignoring routing keys. Catch all.
                                channel.QueueDeclare(deadLetterQueue, true, false, false, null);
                                channel.QueueBind(deadLetterQueue, deadLetterExchange, deadLetterQueue);

                            }
                            else
                            {
                                queueDeclare = channel.QueueDeclare(subscription.Queue, true, false, false, null);
                            }



                            using (var passiveChannel = connection.CreateModel())
                            {
                                try
                                {
                                    passiveChannel.ExchangeDeclarePassive(subscription.Exchange);
                                    passiveChannel.QueueDeclarePassive(subscription.Queue);
                                    passiveChannel.Close();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Exchange or Queue dose not exist");
                                    passiveChannel.Close();
                                    connection.Close();
                                    break;
                                }

                            }


                            //Bind to Queue

                            channel.QueueBind(subscription.Queue, subscription.Exchange, subscription.EventType);




                            var consumer = new AsyncEventingBasicConsumer(channel);

                            _logger.LogInformation($"Start Listening ...[{subscription.Identifier}]");

                            consumer.Received += async (model, ea) =>
                            {

                                var body = ea.Body;
                                var rawMessage = Encoding.UTF8.GetString(body);
                                _logger.LogInformation($"Message Received : [{rawMessage}]");

                                Message messageObj = null;
                                messageObj = ParseMessage(rawMessage);

                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var processor =
                                        (IMessageProcessor)scope.ServiceProvider.GetRequiredService(
                                            subscription
                                                .ProcessorType);
                                    IMessageControlService messageControlSrv =
                                        scope.ServiceProvider.GetService<IMessageControlService>();

                                    try
                                    {
                                            // Let the MessageController know know we got a message.
                                            messageControlSrv?.SendMessageControl(MessageState.Received, messageObj, subscription);

                                        var result = await processor.ProcessAsync(messageObj);

                                        switch (result.Status)
                                        {
                                            case ResultStatus.Success:

                                                channel.BasicAck(ea.DeliveryTag, false);
                                                    // Let the MessageController know we have a failure.
                                                    messageControlSrv?.SendMessageControl(MessageState.Success, messageObj, subscription, result.Response);

                                                break;
                                            case ResultStatus.Retry:

                                                _logger.LogWarning(
                                                    $"SubscriptionID: {subscription.Identifier} - Returned Retry from Processor for MessageID: {messageObj.MessageID}");

                                                channel.BasicNack(ea.DeliveryTag, false, false);
                                                messageControlSrv?.SendMessageControl(MessageState.Retry, messageObj, subscription, result.Response);

                                                break;
                                            case ResultStatus.Failure:

                                                _logger.LogWarning(
                                                    $"SubscriptionID: {subscription.Identifier} - Returned Failure from Processor for MessageID: {messageObj.MessageID}");

                                                channel.BasicNack(ea.DeliveryTag, false, false);
                                                messageControlSrv?.SendMessageControl(MessageState.Failure, messageObj, subscription, result.Response);

                                                break;

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex,
                                            $"SubscriptionId: [{subscription.Identifier}] - Error reported from messageTarget for MessageID: [{messageObj?.MessageID}]");

                                        channel.BasicNack(ea.DeliveryTag, false, false);
                                        messageControlSrv?.SendMessageControl(MessageState.Failure, messageObj, subscription, ex);
                                    }

                                }

                            };

                            channel.BasicConsume(queue: queueDeclare.QueueName,
                                autoAck: false,
                                consumer: consumer);

                            subscription.IsRunning = true;


                            await Task.WhenAny(Task.Delay(Timeout.Infinite, cancellationToken));
                        }
                    }
                    catch (EndOfStreamException ex)
                    {
                        _logger.LogError(ex,
                            $"SubscriptionID: {subscription.Identifier} - An EndOfStreamException occurred with the rabbit queue process. We will not recover, and remove the subscriptions.");
                        break;
                    }
                    catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                    {
                        _logger.LogError(ex,
                            $"SubscriptionID: {subscription.Identifier} - An IOException occurred with the rabbit queue process. We will attempt to recover in {_queueClient.Options.SystemRecoveryInterval} seconds.");
                        await Task.Delay(_queueClient.Options.SystemRecoveryInterval * 1000, cancellationToken);
                    }
                    catch (IOException ex)
                    {
                        _logger.LogError(ex,
                            $"SubscriptionID: {subscription.Identifier} - An IOException occurred with the rabbit queue process. We will attempt to recover in {_queueClient.Options.SystemRecoveryInterval} seconds.");
                        await Task.Delay(_queueClient.Options.SystemRecoveryInterval * 1000, cancellationToken);
                    }

                }



                _logger.LogInformation($"Stop Subscription [{subscription.Identifier}]...");

            });

        }

        private Message ParseMessage(string rawMessage)
        {
            try
            {

                var messageObj = JsonConvert.DeserializeObject<Message>(rawMessage);

                return messageObj;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to parse the Message it is not typeof [{typeof(Message).FullName}] , {Environment.NewLine} [{rawMessage}]");

                return null;
            }

        }
    }
}