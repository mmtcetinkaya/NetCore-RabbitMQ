using System.ComponentModel;

namespace Rabbitmq.Utilities
{
    public static class RabbitMQConsts
    {
        public static int MessagesTTL { get; set; } = 1000 * 60 * 60 * 2;

        public static ushort ParallelThreadsCount { get; set; } = 3;

        public enum ExchangeTypes
        {
            [Description("direct")]
            direct = 1,
            [Description("fanout")]
            fanout = 2,
            [Description("topic")]
            topic = 3,
            [Description("headers")]
            headers = 4
        }
    }
}
