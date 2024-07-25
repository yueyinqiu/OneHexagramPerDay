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

    private static Task DelayUntil(DateTime dateTime)
    {
        var difference = dateTime - DateTime.Now;
        return Task.Delay(difference);
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
            Console.WriteLine("检验完成。将从明天开始正式运行。");

            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            for (; ; )
            {
                date = date.AddDays(1);

                for (int i = 0; i <= 6; i += 2)
                {
                    var next = date.ToDateTime(new TimeOnly(i, 0));
                    Console.WriteLine($"下次触发心跳：{next}");
                    await DelayUntil(next);
                    await mail.SendHeartbeatAsync();
                    Console.WriteLine($"心跳完成。");
                }

                string draft;

                {
                    var next = date.ToDateTime(new TimeOnly(6, 15));
                    Console.WriteLine($"即将发布草稿：{next}");
                    await DelayUntil(next);

                    _ = await weChat.GetTokenAsync(true);
                    draft = await weChat.UploadDraft(zhouyi, date);
                    Console.WriteLine($"草稿完成。");
                }

                {
                    var next = date.ToDateTime(new TimeOnly(6, 30));
                    Console.WriteLine($"即将发布：{next}");
                    await DelayUntil(next);

                    await weChat.Publish(draft);
                    Console.WriteLine($"发布完成。");
                }

                for (int i = 8; i <= 22; i += 2)
                {
                    var next = date.ToDateTime(new TimeOnly(i, 0));
                    Console.WriteLine($"下次触发心跳：{next}");
                    await DelayUntil(next);
                    await mail.SendHeartbeatAsync();
                    Console.WriteLine($"心跳完成。");
                }
            }
        }
        catch (Exception ex)
        {
            await mail.SendExceptionAsync(ex);
            Console.WriteLine("出现错误。内容已发送到邮箱。程序已停止。");
            return;
        }
    }
}