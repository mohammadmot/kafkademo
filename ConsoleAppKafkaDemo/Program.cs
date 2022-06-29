using ConsoleAppKafkaDemo.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// create and run hosted services
CreateHostedBuilder(args).Build().Run();



#region create services
static IHostBuilder CreateHostedBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, collection) =>
    {
        collection.AddSingleton<ISyncThreads,SyncThreads>();

        collection.AddHostedService<ConsumerHostedService>();
        
        collection.AddHostedService<ProducerHostedService>();
    });
#endregion


#region singletone sync threads
public interface ISyncThreads
{
    AutoResetEvent resetEvent { get; set; }  
}

public class SyncThreads : ISyncThreads
{
    public SyncThreads()
    {
        resetEvent = new AutoResetEvent(true);
    }
    public AutoResetEvent resetEvent { get; set; }

}
#endregion