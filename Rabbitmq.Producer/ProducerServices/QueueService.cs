using Rabbitmq.Producer.ProducerInterfaces;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using RabbitMQ.Client;

namespace Rabbitmq.Producer.ProducerServices
{
    public class QueueService : IQueueService
    {
        private readonly IRabbitMQService _rabbitMQServices;
        private IConnection connection;
        private IModel channel;

        public QueueService(IRabbitMQService rabbitMQServices)
        {
            _rabbitMQServices = rabbitMQServices;
            connection = _rabbitMQServices.GetConnection();
            channel = _rabbitMQServices.GetModel(connection);
        }

        public void DeclareExchange(string exchangeName, string exchangeType, bool durable = false, bool autoDelete = false, IDictionary<string, object>? arguments = null)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable, autoDelete, arguments);
        }

        public QueueDeclareOk DeclareQueue(string queueName, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments = null)
        {
            return channel.QueueDeclare(queue: queueName,
                           durable: durable,      // ile in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir.
                           exclusive: exclusive,   // yalnızca bir bağlantı tarafından kullanılır ve bu bağlantı kapandığında sıra silinir - özel olarak işaretlenirse silinmez
                           autoDelete: autoDelete,  // en son bir abonelik iptal edildiğinde en az bir müşteriye sahip olan kuyruk silinir
                           arguments);   // isteğe bağlı; eklentiler tarafından kullanılır ve TTL mesajı, kuyruk uzunluğu sınırı, vb. 
        }

        public void QueueBind(string queueName, string exchangeName, string routingKey, IDictionary<string, object>? arguments = null)
        {
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey, arguments);
        }
    }
}
