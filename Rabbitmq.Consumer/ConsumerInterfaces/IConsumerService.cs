namespace Rabbitmq.Consumer.ConsumerInterfaces
{
    public interface IConsumerService
    {
        Task Consume(string? queueName);
    }
}
