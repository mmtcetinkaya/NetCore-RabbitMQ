﻿using Microsoft.Extensions.Options;
using Rabbitmq.Utilities.RabbitmqConfiguration;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Rabbitmq.Utilities.RabbitmqServices
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQConfigurationOptions _rabbitMQServiceOptions;

        public RabbitMQService(IOptions<RabbitMQConfigurationOptions> rabbitMQServiceOptions)
        {
            _rabbitMQServiceOptions = rabbitMQServiceOptions.Value;
        }

        public IConnection GetConnection()
        {
            try
            {
                Console.WriteLine("trying to connect rabbitmq");
                Console.WriteLine(_rabbitMQServiceOptions.HostName + ", " + _rabbitMQServiceOptions.UserName + ", "+ _rabbitMQServiceOptions.Password + ", "+ _rabbitMQServiceOptions.Port);
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQServiceOptions.HostName,
                    UserName = _rabbitMQServiceOptions.UserName,
                    Password = _rabbitMQServiceOptions.Password,
                    Port = Convert.ToInt32(_rabbitMQServiceOptions.Port)
                };

                factory.AutomaticRecoveryEnabled = true;
                factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
                factory.TopologyRecoveryEnabled = false;

                Console.WriteLine("connection Starting");
                return factory.CreateConnection();
            }
            catch (BrokerUnreachableException ex )
            {
                Console.WriteLine("failed connect rabbit mq");
                Console.WriteLine(ex.Message + ex.InnerException != null ? ex.InnerException?.Message : "");

                throw;
            }
        }

        public IModel GetModel(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}
