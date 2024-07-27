using System.Net;
using System.Net.Mail;

namespace WeChatPublisher;
public sealed class MailSender(
    string address, string password,
    string displayName = "OneHexagramPerDay WeChatPublisher",
    string host = "smtp-mail.outlook.com",
    int port = 587)
{
    private async Task SendAsync(string subject, string body)
    {
        using var client = new SmtpClient(host, port)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(address, password)
        };
        await client.SendMailAsync(new MailMessage()
        {
            From = new MailAddress(address, displayName),
            To = { new MailAddress(address, address) },
            Subject = subject,
            Body = body
        });
    }

    public Task SendStartupMessageAsync()
    {
        return this.SendAsync(
            "程序启动 - OneHexagramPerDay WeChatPublisher",
            "这封邮件是用以测试相关功能是否正常的。");
    }

    public Task SendExceptionAsync(Exception body)
    {
        return this.SendAsync(
            "异常 - OneHexagramPerDay WeChatPublisher",
            body.ToString());
    }

    public Task SendHeartbeatAsync(DateTime body)
    {
        return this.SendAsync(
            "心跳 - OneHexagramPerDay WeChatPublisher",
            $"来自 OneHexagramPerDay WeChatPublisher 的心跳包。期望时间为 {body} 。");
    }
}
