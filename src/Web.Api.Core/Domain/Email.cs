namespace Web.Api.Core.Domain;
public class Email
{
    public string Recipient { get; }
    public string Subject { get; private set; }
    public string TextBody { get; private set; }
    public string HtmlBody { get; private set; }
    public Email(string recipient, string subject) => (Recipient, Subject) = (recipient, subject);
    public Email(string recipient, string subject, string message) => (Recipient, Subject, HtmlBody) = (recipient, subject, Build(message));
    private static string Build(string message)
    {
        return $"<h2>{message}</h2>";
    }
}