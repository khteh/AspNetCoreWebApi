namespace Web.Api.Core.Configuration;

public class EmailSettings
{
    public required string Server { get; set; }
    public int Port { get; set; }
    public bool DefaultCredentials { get; set; }
    public required string SenderName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public bool TLS { get; set; }
}