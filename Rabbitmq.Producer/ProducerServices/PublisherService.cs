using Rabbitmq.Producer.ProducerInterfaces;
using Rabbitmq.Utilities.MessageObjects;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rabbitmq.Producer.ProducerServices
{
    public class PublisherService : IPublisherService
    {
        private readonly IRabbitMQService _rabbitMQServices;
        private readonly IQueueService _queueServices;


        public PublisherService(IRabbitMQService rabbitMQServices, IQueueService queueServices)
        {
            _rabbitMQServices = rabbitMQServices;
            _queueServices = queueServices;
        }

        public void DirectEnqueu<T>(string exchangeName, string routingKey, IEnumerable<T> messages)
        {
            try
            {
                IConnection connection = _rabbitMQServices.GetConnection();
                IModel channel = _rabbitMQServices.GetModel(connection);

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    channel.BasicPublish(exchange: exchangeName,
                                                 routingKey: routingKey,
                                                 mandatory: false,
                                                 body: body);
                }
            }

            catch (Exception ex)
            {
                //loglama yapılabilir
                throw new Exception(ex.Message + " " + ex.InnerException?.Message.ToString());
            }
        }

        public void FanoutEnqueu<T>(string exchangeName, IEnumerable<T> messages)
        {
            try
            {
                IConnection connection = _rabbitMQServices.GetConnection();
                IModel channel = _rabbitMQServices.GetModel(connection);

                foreach (var message in messages)
                {
                    var options1 = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, options1));

                    // RabbitMQservis durup yeninden başaltıldığında mesajların kalması gerekiyorsa persistent: true olmalı
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: exchangeName,
                                                 routingKey: "",
                                                 basicProperties: properties,
                                                 body: body);
                }
            }

            catch (Exception ex)
            {
                //loglama yapılabilir
                throw new Exception(ex.Message + " " + ex.InnerException?.Message.ToString());
            }
        }

        public void HeaderEnqueu<T>(string exchangeName, IEnumerable<T> messages, Dictionary<string, object> headerProps, IBasicProperties? basicProperties = null)
        {
            try
            {
                IConnection connection = _rabbitMQServices.GetConnection();
                IModel channel = _rabbitMQServices.GetModel(connection);

                foreach (var message in messages)
                {
                    var options1 = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, options1));

                    if (basicProperties == null)
                    {
                        basicProperties = channel.CreateBasicProperties();
                    }

                    basicProperties.Headers = headerProps;

                    channel.BasicPublish(exchange: exchangeName,
                                                 routingKey: "",
                                                 basicProperties: basicProperties,
                                                 body: body);
                }
            }

            catch (Exception ex)
            {
                //loglama yapılabilir
                throw new Exception(ex.Message + " " + ex.InnerException?.Message.ToString());
            }
        }

        public void TopicEnqueu(string exchangeName, List<TopicMessage> messageList)
        {
            try
            {
                IConnection connection = _rabbitMQServices.GetConnection();
                IModel channel = _rabbitMQServices.GetModel(connection);

                foreach (var message in messageList)
                {
                    var options1 = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message.Message, options1));

                    channel.BasicPublish(exchange: exchangeName,
                                                 routingKey: message.RoutingKey,
                                                 body: body);
                }
            }

            catch (Exception ex)
            {
                //loglama yapılabilir
                throw new Exception(ex.Message + " " + ex.InnerException?.Message.ToString());
            }
        }
    }
}
