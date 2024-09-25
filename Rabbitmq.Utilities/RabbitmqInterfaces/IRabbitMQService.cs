using RabbitMQ.Client;

namespace Rabbitmq.Utilities.RabbitmqInterfaces
{
    public interface IRabbitMQService
    {
        IConnection GetConnection();
        IModel GetModel(IConnection connection);
    }
}
