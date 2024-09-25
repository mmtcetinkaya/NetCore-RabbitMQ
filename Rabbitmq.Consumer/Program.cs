using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rabbitmq.Consumer.ConsumerInterfaces;
using Rabbitmq.Consumer.ConsumerServices;
using Rabbitmq.Utilities.RabbitmqConfiguration;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using Rabbitmq.Utilities.RabbitmqServices;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Consumer starts reading...");
Console.WriteLine("--------------------------------------------------------------");

string? queueName = "";
while (queueName != "quit")
{
    Console.WriteLine("Enter queue name to listen or 'quit' to exit");
    queueName = Console.ReadLine();

    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    builder.Services.Configure<RabbitMQConfigurationOptions>(builder.Configuration.GetSection(typeof(RabbitMQConfigurationOptions).Name));
    builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
    builder.Services.AddScoped<IConsumerService, ConsumerService>();

    var app = builder.Build();
    var _cunsomerService = app.Services.GetRequiredService<IConsumerService>();
    var configurationFile = app.Services.GetRequiredService<IOptions<RabbitMQConfigurationOptions>>();

    await _cunsomerService.Consume(queueName);

    Console.WriteLine($"ConsumerService bitti:  {DateTime.Now.ToShortTimeString()}");
}



