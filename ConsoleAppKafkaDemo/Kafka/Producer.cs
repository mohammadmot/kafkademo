using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleAppKafkaDemo.Kafka
{
    public class ProducerHostedService : IHostedService
    {
        readonly ILogger _logger;
        // private IProducer<int, string> _producer;
        private IProducer<Null, string> _producer;
        private readonly ISyncThreads _syncThreads;

        public ProducerHostedService(ILogger<ProducerHostedService> logger, ISyncThreads syncThreads)
        {
            _logger = logger;
            _syncThreads = syncThreads;

            var config = new ProducerConfig()
            {
                BootstrapServers = "31.7.67.227:29092"
            };

            // _producer = new ProducerBuilder<int, string>(config).Build();
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public ILogger<ProducerHostedService> Logger { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // wait on consumer to start
            _syncThreads.resetEvent.WaitOne();

            for (int i = 0; i < 10; i++)
            {
                var value = $"Message {i}";
                _logger.LogInformation($"Sent: {value}");

                var result = await _producer.ProduceAsync(
                    "Topic1",
                    // new Message<int, string> { Key = i, Value = value }
                    new Message<Null, string> { Value = value }
                    , cancellationToken);

                int nQueueCount = _producer.Flush(TimeSpan.FromSeconds(10));
                _logger.LogInformation($"Kafka Producer > QueueCount:{nQueueCount},Offset:{result.Offset},Status:{result.Status}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // ...
            return Task.CompletedTask;
        }
    }
}
