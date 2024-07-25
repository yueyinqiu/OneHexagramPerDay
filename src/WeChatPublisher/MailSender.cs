using System.Net;
using System.Net.Mail;

namespace WeChatPublisher;
public sealed class MailSender : IDisposable
{
    private readonly SmtpClient client;
    private readonly string userName;

    public MailSender(
        string userName,
        string password,
        string smtpAddress,
        int? smtpPort = null)
    {
        this.client = smtpPort.HasValue ?
           new SmtpClient(smtpAddress, smtpPort.Value) :
           new SmtpClient(smtpAddress);
        this.client.UseDefaultCredentials = false;
        this.client.EnableSsl = true;
        this.client.Credentials = new NetworkCredential(userName, password);
        this.userName = userName;
    }

    public void Dispose() => this.client.Dispose();

    public Task SendAsync(string subject, string body, string? recipients = null)
    {
        recipients ??= this.userName;
        return this.client.SendMailAsync(this.userName, recipients, subject, body);
    }
    public Task SendExceptionAsync(
        Exception body,
        string subject = "异常 - OneHexagramPerDay WeChatPublisher",
        string? recipients = null)
    {
        recipients ??= this.userName;
        return this.client.SendMailAsync(this.userName, recipients, subject, body.ToString());
    }
    public Task SendHeartbeatAsync(
        string subject = "心跳 - OneHexagramPerDay WeChatPublisher",
        string body = "OneHexagramPerDay WeChatPublisher 的心跳包",
        string? recipients = null)
    {
        recipients ??= this.userName;
        return this.client.SendMailAsync(this.userName, recipients, subject, body.ToString());
    }
}
