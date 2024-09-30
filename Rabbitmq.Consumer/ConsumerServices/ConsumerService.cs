using Rabbitmq.Consumer.ConsumerInterfaces;
using Rabbitmq.Utilities;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Rabbitmq.Consumer.ConsumerServices
{
    public class ConsumerService : IConsumerService
    {
        private SemaphoreSlim _semaphore;
        private IModel channel;
        private readonly IRabbitMQService _rabbitMQServices;

        public ConsumerService(IRabbitMQService rabbitMQServices)
        {
            _rabbitMQServices = rabbitMQServices;
        }

        public async Task Consume(string? queueName)
        {
            try
            {
                _semaphore = new SemaphoreSlim(RabbitMQConsts.ParallelThreadsCount);

                IConnection connection = _rabbitMQServices.GetConnection();
                channel = _rabbitMQServices.GetModel(connection);

                channel.BasicQos(0, RabbitMQConsts.ParallelThreadsCount, false);
                var _consumer = new EventingBasicConsumer(channel);

                await Task.FromResult(channel.BasicConsume(queue: queueName,
                                        autoAck: false,
                                        consumer: _consumer));

                _consumer.Received += Consumer_Received;
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.InnerException?.Message);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                _semaphore.Wait();
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" Received " + message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException.Message.ToString());
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    _semaphore.Release();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.InnerException?.Message);
            }
        }
    }
}