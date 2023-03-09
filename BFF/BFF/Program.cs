using BFF.Configuration;
using BFF.Extensions;
using Paramore.Brighter.MessagingGateway.Kafka;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) //load base settings
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true) //load environment settings    
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true) //load local settings
    .AddEnvironmentVariables();

var brokerSettings = builder.Configuration.GetRequiredSection(BrokerSettingsOptions.BrokerSettings).Get<BrokerSettingsOptions>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var kafkaGatewayConfig = new KafkaMessagingGatewayConfiguration()
{
    Name = "eda.chatapp",
    BootStrapServers = new[] { $"{brokerSettings!.Host}:{brokerSettings.Port}" }
};

builder.Services.AddBrighter( kafkaGatewayConfig );

builder.Services.AddGraphQL();

var app = builder.Build();
app.Logger.LogInformation($"Kafka target is {brokerSettings.Host}:{brokerSettings.Port}");

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

app.ConfigureGraphQL();

app.Run();