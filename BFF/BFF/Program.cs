using BFF.Models;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.Transforms.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var kafkaGatewayConfig = new KafkaMessagingGatewayConfiguration()
{
    Name = "eda.chatapp",
    BootStrapServers = new[] { "localhost:9092" }
};

// Brighter configuration: 
// https://brightercommand.gitbook.io/paramore-brighter-documentation/brighter-configuration/brighterbasicconfiguration
builder.Services
    .AddBrighter(
        options => {
            options.RequestContextFactory = new InMemoryRequestContextFactory();
            options.HandlerLifetime = ServiceLifetime.Scoped;
            options.CommandProcessorLifetime = ServiceLifetime.Scoped;
            options.MapperLifetime = ServiceLifetime.Singleton;
        }
    )
    .AutoFromAssemblies()
    .UseExternalBus(
        new KafkaProducerRegistryFactory(
            kafkaGatewayConfig,
            new KafkaPublication[] { new KafkaPublication() 
            {
                Topic = new RoutingKey("ChatMessages"),
                MakeChannels = OnMissingChannel.Create,
                TransactionalId = Guid.NewGuid().ToString()
            }
            }
        ).Create()
    );

builder.Services.AddServiceActivator( options => {
    options.Subscriptions = new KafkaSubscription[] 
    {
        new KafkaSubscription<BFF.Controllers.ChatMessageSent>(
            name: new SubscriptionName("eda.chatapp"),
            channelName: new ChannelName("ChatMessages"),
            routingKey: new RoutingKey("ChatMessages")
        )
    };
    options.ChannelFactory = new ChannelFactory( new KafkaMessageConsumerFactory( kafkaGatewayConfig ));
}).AutoFromAssemblies();

builder.Services.AddSingleton<IAmAStorageProviderAsync, InMemoryStorageProviderAsync>();
builder.Services.AddSingleton<MessageModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Environment is development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else {
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();