namespace Web.Api.Core.Configuration;
public class EmailSettings
{
    public string Server { get; set; }
    public int Port { get; set; }
    public bool DefaultCredentials { get; set; }
    public string SenderName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool TLS { get; set; }
}