namespace Rabbitmq.Utilities.MessageObjects
{
    public class HeaderMessage
    {
        public string Message { get; set; }
        public Dictionary<string, object> Headers { get; set; }
    }
}
