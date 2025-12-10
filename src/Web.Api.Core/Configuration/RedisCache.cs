namespace Web.Api.Core.Configuration;

public class RedisCache
{
    public required string Connection { get; set; }
    public required string InstanceName { get; set; }
    public int SyncTimeout { get; set; }
}