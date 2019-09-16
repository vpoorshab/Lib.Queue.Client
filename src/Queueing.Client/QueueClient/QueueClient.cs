using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lib.Queueing.Client
{
    public class QueueClient : IQueueClient
    {
        private readonly ILogger<QueueClient> _logger;

        private ConnectionFactory _connectionFactory;
        public QueueClientOption Options
        {
            get;
        }

        public ConnectionFactory Factory => _connectionFactory;


        public QueueClient(QueueClientOption queueClientOption, ILogger<QueueClient> logger)
        {

            Options = queueClientOption;
            _logger = logger;
            Initialize(queueClientOption);
        }

        public void Initialize(QueueClientOption queueClientOption)
        {



            try
            {
                _logger.LogInformation("Initiating the Connection Factory");

                _connectionFactory = new ConnectionFactory();

                if (!string.IsNullOrWhiteSpace(queueClientOption.Host))
                {
                    _connectionFactory.HostName = queueClientOption.Host;
                }
                if (!string.IsNullOrWhiteSpace(queueClientOption.UserName))
                {
                    _connectionFactory.UserName = queueClientOption.UserName;
                }

                if (!string.IsNullOrWhiteSpace(queueClientOption.Password))
                {
                    _connectionFactory.Password = queueClientOption.Password;
                }

                if (queueClientOption.Port != 0)
                {
                    _connectionFactory.Port = queueClientOption.Port;
                }


                _connectionFactory.AutomaticRecoveryEnabled = queueClientOption.AutomaticRecoveryEnabled;
                _connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(queueClientOption.NetworkRecoveryInterval);
                _connectionFactory.DispatchConsumersAsync = true;



            }
            catch
            {
                _logger.LogExceptionScope();
                throw;
            }



        }
    }
}
