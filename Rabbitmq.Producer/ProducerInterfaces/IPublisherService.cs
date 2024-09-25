using Rabbitmq.Utilities.MessageObjects;
using RabbitMQ.Client;

namespace Rabbitmq.Producer.ProducerInterfaces
{
    public interface IPublisherService
    {
        void DirectEnqueu<T>(string exchangeName, string routingKey, IEnumerable<T> messages);
        void FanoutEnqueu<T>(string exchangeName, IEnumerable<T> messages);
        void HeaderEnqueu<T>(string exchangeName, IEnumerable<T> messages, Dictionary<string, object> headerProps, IBasicProperties? basicProperties = null);
        void TopicEnqueu(string exchangeName, List<TopicMessage> messageList);
    }
}
