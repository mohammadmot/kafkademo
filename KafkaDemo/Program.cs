using Confluent.Kafka;
using KafkaDemo.Kafka;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// create and run hosted services
CreateHostedBuilder(args).Build().Run();

app.Run();


static IHostBuilder CreateHostedBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, collection) =>
    {
        collection.AddHostedService<ProducerHostedService>();
        // collection.AddSingleton(typeof());
    });