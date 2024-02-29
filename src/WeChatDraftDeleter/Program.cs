using System.Net.Http.Json;
using System.Text.Json.Nodes;
using WeChatDraftAdder.Extensions;

using HttpClient client = new HttpClient();

#region 一、请求令牌
string accessToken;
{
    Console.WriteLine("一、请求令牌");

    Console.Write("请输入 AppID ：");
    var appId = Console.ReadLine();
    Console.Write("请输入 AppSecret ：");
    var appSecret = Console.ReadLine();

    Console.WriteLine("正在请求令牌……");
    var response = await client.GetFromJsonAsync<JsonObject>(
        $"https://api.weixin.qq.com/cgi-bin/token?" +
        $"grant_type=client_credential&" +
        $"appid={appId}&" +
        $"secret={appSecret}");

    var result = response?["access_token"]?.GetValue<string>();
    if (result is null)
    {
        Console.WriteLine("令牌解析失败，具体响应为：");
        Console.WriteLine(response);
        return;
    }
    accessToken = result;

    Console.WriteLine($"成功获取令牌 {accessToken} 。");
    Console.WriteLine();
}
#endregion

#region 二、确认草稿
Dictionary<string, List<string>> drafts;
{
    Console.WriteLine("二、确认草稿");

    Console.WriteLine($"正在请求草稿列表……");
    drafts = [];
    for (int pageIndex = 0; ; pageIndex++)
    {
        const int pageSize = 20;

        Console.WriteLine($"正在请求草稿列表（第 {pageIndex + 1} 页）……");
        var response = await client.PostAsWeChatThenFromJson<dynamic, JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/draft/batchget?" +
            $"access_token={accessToken}",
            new
            {
                offset = pageIndex * pageSize,
                count = pageSize,
                no_content = 1
            });

        var items = response?["item"]?.AsArray();
        foreach (var item in items!)
        {
            var titles = new List<string>();
            drafts.Add((string)item?["media_id"]?.AsValue()!, titles);
            var news_items = item?["content"]?["news_item"]?.AsArray();
            foreach (var news_item in news_items!)
                titles.Add((string)(news_item?["title"]?.AsValue()!));
        }
        if (items.Count < pageSize)
            break;
    }

    Console.WriteLine($"成功获取到 {drafts.Count} 篇草稿：");
    foreach (var (k, v) in drafts)
        Console.WriteLine($"    {k} ：{string.Join('、', v)}");
    Console.Write($"它们将被全部删除，在确认后请按回车继续：");
    var line = Console.ReadLine();
    if (line is null)
        return;
    Console.WriteLine();
}
#endregion

#region 三、删除草稿
{
    Console.WriteLine("三、删除草稿");

    foreach (var ((draft, _), i) in drafts.Select((d, i) => (d, i + 1)))
    {
        Console.WriteLine($"正在删除草稿 {draft} （ {i}/{drafts.Count} ）……");
        var request = new
        {
            media_id = draft
        };
        var response = await client.PostAsWeChatThenFromJson<dynamic, JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/draft/delete?" +
            $"access_token={accessToken}",
            request);
        if ((int?)response?["errcode"]?.AsValue() != 0)
        {
            Console.WriteLine("草稿删除失败，具体响应为：");
            Console.WriteLine(response);
            return;
        }
    }
    Console.WriteLine($"已全部完成。");
    Console.WriteLine();
}
#endregion
