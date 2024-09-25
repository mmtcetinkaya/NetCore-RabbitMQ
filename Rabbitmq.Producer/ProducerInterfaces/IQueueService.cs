using RabbitMQ.Client;

namespace Rabbitmq.Producer.ProducerInterfaces
{
    public interface IQueueService
    {
        QueueDeclareOk DeclareQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments = null);
        void DeclareExchange(string exchange, string type, bool durable = false, bool autoDelete = false, IDictionary<string, object>? arguments = null);
        void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object>? arguments = null);
    }
}
