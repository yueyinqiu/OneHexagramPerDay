using System.Net;
using System.Net.Mail;

namespace WeChatPublisher;
public sealed class MailSender(
    string userName, string password, string smtpAddress, int? smtpPort = null)
{
    public async Task SendAsync(string subject, string body, string? recipients = null)
    {
        using var client = smtpPort.HasValue ?
           new SmtpClient(smtpAddress, smtpPort.Value) :
           new SmtpClient(smtpAddress);
        this.client.UseDefaultCredentials = false;
        this.client.EnableSsl = true;
        this.client.Credentials = new NetworkCredential(userName, password);

        recipients ??= userName;
        try
        {
            await this.client.SendMailAsync(userName, recipients, subject, body);
        }
        catch
        {
            // 重试一次
            await Task.Delay(TimeSpan.FromSeconds(10));
            await this.client.SendMailAsync(userName, recipients, subject, body);
        }
    }

    public Task SendStartupMessageAsync(
        string subject = "程序启动 - OneHexagramPerDay WeChatPublisher",
        string body = "这封邮件是用以测试相关功能是否正常的。",
        string? recipients = null)
    {
        return SendAsync(subject, body.ToString(), recipients);
    }

    public Task SendExceptionAsync(
        Exception body,
        string subject = "异常 - OneHexagramPerDay WeChatPublisher",
        string? recipients = null)
    {
        return SendAsync(subject, body.ToString(), recipients);
    }

    public Task SendHeartbeatAsync(
        DateTime body,
        string subject = "心跳 - OneHexagramPerDay WeChatPublisher",
        string? recipients = null)
    {
        return SendAsync(
            subject, 
            $"来自 OneHexagramPerDay WeChatPublisher 的心跳包。期望时间为 {body} 。",
            recipients);
    }
}
