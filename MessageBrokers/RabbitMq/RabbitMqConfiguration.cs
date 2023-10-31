namespace MessageBrokers.RabbitMq
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; }

        public string? Exchange { get; set; }

        public string QueueName { get; set; }

        public byte DeliveryMode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public int ReconnectDelay { get; set; } = 5000;
        public int ReconnectAttemptsCount { get; set; }
    }
}