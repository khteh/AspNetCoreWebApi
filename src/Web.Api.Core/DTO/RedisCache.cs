namespace Web.Api.Core.DTO
{
    public class RedisCache
    {
        public string Connection {get; set;}
        public string InstanceName {get; set;}
        public int SyncTimeout {get; set;}
    }
}