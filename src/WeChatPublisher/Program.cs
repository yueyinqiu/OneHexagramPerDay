using OneHexagramPerDayCore;
using WeChatPublisher;
using YiJingFramework.Annotating.Zhouyi;

internal class Program
{
    private static MailSender InputMail()
    {
        Console.Write("请输入 Outlook 邮箱账户：");
        var userName = Console.ReadLine();
        Console.Write("请输入 Outlook 邮箱密码：");
        var password = Console.ReadLine();

        return new MailSender(
            userName!,
            password!, "smtp-mail.outlook.com", 587);
    }

    private static WeChatRequester InputWeChat()
    {
        Console.Write("请输入 AppId ：");
        var appId = Console.ReadLine();
        Console.Write("请输入 AppSecret ：");
        var appSecret = Console.ReadLine();

        return new WeChatRequester(appId!, appSecret!);
    }

    private static async Task<bool> DelayUntil(DateTime dateTime)
    {
        var difference = dateTime - DateTime.Now;
        if (difference < TimeSpan.Zero)
            return false;
        await Task.Delay(difference);
        return true;
    }

    private static ZhouyiStoreWithLineTitles LoadZhouyi()
    {
        var storeContent = File.ReadAllText("./zhouyi.json");
        var zhouyiStore = ZhouyiStore.DeserializeFromJsonString(storeContent);
        return new ZhouyiStoreWithLineTitles(zhouyiStore!);
    }

    public static async Task Main()
    {
        using var mail = InputMail();
        try
        {
            await mail.SendAsync(
                "测试邮件 - OneHexagramPerDay WeChatPublisher",
                "这是一封测试邮件。");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return;
        }
        Console.WriteLine("测试邮件发送成功，后续所有错误消息将以邮件形式发送。");

        try
        {
            var zhouyi = LoadZhouyi();
            Console.WriteLine("周易加载成功。");

            using var weChat = InputWeChat();
            _ = await weChat.GetTokenAsync(true);

            Console.Clear();
            Console.WriteLine("检验完成。");

            for (var date = DateOnly.FromDateTime(DateTime.Now); ; date = date.AddDays(1))
            {
                {
                    var next = date.ToDateTime(new TimeOnly(6, 0));
                    Console.WriteLine($"下次触发心跳：{next}");
                    if (await DelayUntil(next))
                    {
                        await mail.SendHeartbeatAsync(next);
                        Console.WriteLine($"心跳完成。");
                    }
                    else
                    {
                        Console.WriteLine($"已跳过。");
                    }
                }

                {
                    var next = date.ToDateTime(new TimeOnly(6, 25));
                    Console.WriteLine($"即将发布草稿：{next}");
                    if (await DelayUntil(next))
                    {
                        _ = await weChat.GetTokenAsync(true);
                        var draft = await weChat.UploadDraft(zhouyi, date);
                        Console.WriteLine($"草稿完成。");

                        next = date.ToDateTime(new TimeOnly(6, 30));
                        Console.WriteLine($"即将发布：{next}");
                        _ = await DelayUntil(next);
                        await weChat.Publish(draft);
                        Console.WriteLine($"发布完成。");
                    }
                    else
                    {
                        Console.WriteLine($"已跳过。");
                    }
                }

                {
                    var next = date.ToDateTime(new TimeOnly(22, 0));
                    Console.WriteLine($"下次触发心跳：{next}");
                    if (await DelayUntil(next))
                    {
                        await mail.SendHeartbeatAsync(next);
                        Console.WriteLine($"心跳完成。");
                    }
                    else
                    {
                        Console.WriteLine($"已跳过。");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            try
            {
                await mail.SendExceptionAsync(ex);
                Console.WriteLine("出现错误。内容已发送到邮箱。程序已停止。");
            }
            catch (Exception mailEx)
            {
                Console.WriteLine("出现错误，且邮件发送失败。出现的错误为：");
                Console.WriteLine(ex);
                Console.WriteLine("邮件发送失败的错误为：");
                Console.WriteLine(mailEx);
                Console.WriteLine("程序已停止。");
            }
            return;
        }
    }
}