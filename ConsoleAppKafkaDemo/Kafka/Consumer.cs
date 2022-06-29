using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ConsoleAppKafkaDemo.Kafka
{
    public class ConsumerHostedService : IHostedService
    {
        readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly ClusterClient _cluster;
        private readonly ISyncThreads _syncThreads;
        private string _allDataReceived;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, ISyncThreads syncThreads)
        {
            _logger = logger;
            _syncThreads = syncThreads;

            _cluster = new ClusterClient( new Configuration 
            {
                Seeds = "31.7.67.227:29092"
            }, new ConsoleLogger());

            _allDataReceived = string.Empty;
        }

        public ILogger<ProducerHostedService> Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("Topic1");

            // ...

            _cluster.MessageReceived += record =>
            {
                string strValue = Encoding.UTF8.GetString(record.Value as byte[]);

                if (record.Key != null)
                _logger.LogInformation($"Received > key:{Encoding.UTF8.GetString(record.Key as byte[])}," +
                    $"value:{strValue}");
                else
                    _logger.LogInformation($"Received > value:{strValue}");

                if (!string.IsNullOrEmpty(strValue))
                {
                    _allDataReceived += strValue + '|';
                }
            };

            // signal to producer to start
            _syncThreads.resetEvent.Set();
            _logger.LogInformation($"Signal > Set event.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            _logger.LogInformation($"Received All Data > value:{_allDataReceived}");

            return Task.CompletedTask;
        }
    }
}
