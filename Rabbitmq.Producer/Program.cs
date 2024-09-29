using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rabbitmq.Producer.ProducerInterfaces;
using Rabbitmq.Producer.ProducerServices;
using Rabbitmq.Utilities.MessageObjects;
using Rabbitmq.Utilities.RabbitmqConfiguration;
using Rabbitmq.Utilities.RabbitmqInterfaces;
using Rabbitmq.Utilities.RabbitmqServices;
using static Rabbitmq.Utilities.RabbitMQConsts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string? environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine("runing environment :" + environmentName);
builder.Configuration
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.Configure<RabbitMQConfigurationOptions>(builder.Configuration.GetSection(typeof(RabbitMQConfigurationOptions).Name));

builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();


var app = builder.Build();
var _publisherService = app.Services.GetRequiredService<IPublisherService>();
var _queueService = app.Services.GetRequiredService<IQueueService>();

var configurationFile = app.Services.GetRequiredService<IOptions<RabbitMQConfigurationOptions>>();

// Direct
string directExchangeName = "direct-exchange";
string directQueueName = "direct-example-queue";
string rountingKey = "key1";

_queueService.DeclareExchange(exchange: directExchangeName, type: ExchangeTypes.direct.ToString(), durable: true);

_queueService.DeclareQueue(queue: directQueueName,
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

_queueService.QueueBind(queue: directQueueName,
                        exchange: directExchangeName,
                        routingKey: rountingKey);

List<string> directMessages = new List<string> { "deneme 1", "deneme 2", "deneme 3" };
_publisherService.DirectEnqueu(directExchangeName, rountingKey, directMessages);

//Fanout
string fanoutExchangeName = "fanout-exchange";

_queueService.DeclareExchange(fanoutExchangeName, ExchangeTypes.fanout.ToString(), true);

List<string> fanoutQueueList = new List<string> { "fanout-queue-1", "fanout-queue-2", "fanout-queue-3" };

foreach (var queueName in fanoutQueueList)
{
    _queueService.DeclareQueue(queue: queueName,
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    _queueService.QueueBind(queueName, fanoutExchangeName, "");
}

List<string> messages = new List<string> { "fanout deneme mesajı 1" };
_publisherService.FanoutEnqueu("fanout-exchange", messages);

//Header
string headerExchangeName = "header-exchange";

Dictionary<string, Dictionary<string, object>> headerQueueList = new Dictionary<string, Dictionary<string, object>>
{
    {"header-queue-1", new Dictionary<string, object> {
                                                        { "x-match", "any" }, // One of the headers is match it works
                                                        { "extension", ".pdf" },
                                                        { "content", "receipt" }
                                                      }
    },
    {"header-queue-2", new Dictionary<string, object> {
                                                        { "x-match", "all" }, // All of the headers must match
                                                        { "extension", ".pdf" },
                                                        { "content", "personel-informations" }
                                                      }
    }
};

_queueService.DeclareExchange(headerExchangeName, ExchangeTypes.headers.ToString(), true);

foreach (var queue in headerQueueList)
{
    _queueService.DeclareQueue(queue: queue.Key,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null); 

    _queueService.QueueBind(queue.Key, headerExchangeName, "", queue.Value);
}


List<string> headerMessages = new List<string> { "header deneme mesajı 1" };
var headerMessageProps = new Dictionary<string, object> {
                                                          { "extension", ".pdf" },
                                                          { "content", "personel-informations" }
                                                         };

_publisherService.HeaderEnqueu(headerExchangeName, headerMessages, headerMessageProps);

//Topic
string topicExchangeName = "topic-exchange";

Dictionary<string, string> topicQueueList = new Dictionary<string, string>
{
    {"topic-queue-1","topic.*" },
    {"topic-queue-2","topic.deneme.*" }
};

_queueService.DeclareExchange(topicExchangeName, ExchangeTypes.topic.ToString(), true);

foreach(var topic in topicQueueList)
{

    _queueService.DeclareQueue(queue: topic.Key,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

    _queueService.QueueBind(topic.Key, topicExchangeName, topic.Value);
}

List<TopicMessage> topicMessages = [new TopicMessage { Message = "topic deneme mesajı 1", RoutingKey = "topic.deneme.message" },
                                    new TopicMessage { Message = "topic deneme mesajı 2", RoutingKey = "topic." }];

_publisherService.TopicEnqueu(topicExchangeName, topicMessages);

Console.WriteLine("Publish Completed!");

//pr