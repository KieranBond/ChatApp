using BFF.Controllers;
using BFF.Models;

namespace BFF.Extensions;

public static class GraphQLExtensions
{
    public static void AddGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddInMemorySubscriptions()
            .AddQueryType<ChatMessageQuery>()
            .AddSubscriptionType<ChatMessageSubscription>();
    }

    public static void ConfigureGraphQL( this IApplicationBuilder app )
    {
        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints( endpoints => {
            endpoints.MapGraphQL();
        });
    }
}

public class ChatMessageQuery 
{
    public ChatMessage GetLatestChatMessage([Service]MessageModel model) => model.Get().LastOrDefault() ?? new ChatMessage(string.Empty, string.Empty);
}

public class ChatMessageSubscription
{
    [Subscribe]
    [Topic(nameof(ChatMessage))]
    public ChatMessage OnPublished([EventMessage] ChatMessage message)
    {
        return message;
    }
}