using BFF.Models;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.Transforms.Storage;

namespace BFF.Extensions;

public static class BrighterExtensions
{
    public static void AddBrighter(this IServiceCollection services, KafkaMessagingGatewayConfiguration kafkaGatewayConfig ) 
    {
        // Brighter configuration: 
        // https://brightercommand.gitbook.io/paramore-brighter-documentation/brighter-configuration/brighterbasicconfiguration
        services
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

        services.AddServiceActivator( options => {
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

        services.AddSingleton<IAmAStorageProviderAsync, InMemoryStorageProviderAsync>();
        services.AddSingleton<MessageModel>();
    }
}