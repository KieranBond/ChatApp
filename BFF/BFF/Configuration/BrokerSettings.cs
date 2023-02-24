namespace BFF.Configuration;

public class BrokerSettingsOptions
{
    public const string BrokerSettings = "BrokerSettings";

    public string Host {get;set;} = string.Empty;
    public int Port {get;set;} = -1;
}