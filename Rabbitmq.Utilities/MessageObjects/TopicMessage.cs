namespace Rabbitmq.Utilities.MessageObjects
{
    public class TopicMessage
    {
        public string Message { get; set; }
        public string RoutingKey { get; set; }
    }
}
