using Confluent.Kafka;

namespace KafkaDemo.Kafka
{
    public class ProducerHostedService : IHostedService
    {
        readonly ILogger _logger;
        private IProducer<Null, string> _producer;

        public ProducerHostedService(ILogger<ProducerHostedService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "31.7.67.227:29092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public ILogger<ProducerHostedService> Logger { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Thread.Sleep(5000);
            for (int i = 0; i < 100; i++)
            {
                var value = $"Message {i}";
                _logger.LogInformation(value);

                await _producer.ProduceAsync(
                    "Topic1",
                    new Message<Null, string> { Value = value }
                    , cancellationToken);

                _producer.Flush(TimeSpan.FromSeconds(10));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
