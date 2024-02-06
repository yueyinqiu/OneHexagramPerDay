using OneHexagramPerDayCore;
using System.Data;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using WeChatDraftAdder.Extensions;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;

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
        Console.WriteLine(result);
        return;
    }
    accessToken = result;

    Console.WriteLine($"成功获取令牌 {accessToken} 。");
    Console.WriteLine();
}
#endregion

#region 二、确认起始
List<DateOnly> dates;
{
    Console.WriteLine("二、确认起始");

    var drafts = new List<string>();

    Console.WriteLine($"正在请求草稿列表……");

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
            var news_items = item?["content"]?["news_item"]?.AsArray();
            foreach (var news_item in news_items!)
            {
                var title = (string)(news_item?["title"]?.AsValue()!);
                var nianYueRi = title.Split(' ')[^1];
                drafts.Add(nianYueRi);
            }
        }

        if (items.Count < pageSize)
            break;
    }

    Console.WriteLine($"成功获取到 {drafts.Count} 篇草稿：");
    foreach (var d in drafts)
        Console.WriteLine($"    {d}");

    var today = DateOnly.FromDateTime(DateTime.Today);
    var defaultDate = today;
    for (; ; )
    {
        defaultDate = defaultDate.AddDays(1);
        if (!drafts.Remove(defaultDate.ToNongliLunarString()))
            break;
    }
    Console.WriteLine($"若从明日开始数起，直到" +
        $"{defaultDate: yyyy 年 MM 月 dd 日}（{defaultDate.ToNongliLunarString()}）" +
        $"前均已有相应草稿。此日将被设置为默认的起始日期。");

    Console.Write($"请确认起始日期（默认为 {defaultDate:yyyyMMdd} ）：");
    var line = Console.ReadLine();
    if (!DateOnly.TryParseExact(line, "yyyyMMdd", out var date))
        date = defaultDate;
    Console.WriteLine($"起始日期已设定为 {date:yyyyMMdd} 。");

    Console.Write($"请确认天数（默认为 30 ）：");
    line = Console.ReadLine();
    if (!int.TryParse(line, out var count))
        count = 30;
    Console.WriteLine($"天数已设定为 {count} 。");

    dates = [];
    for (int i = 0; i < count; i++)
    {
        dates.Add(date);
        date = date.AddDays(1);
    }
    dates.Reverse();

    Console.WriteLine($"以下 {dates.Count} 日的草稿将被添加：");
    foreach (var d in dates)
    {
        Console.WriteLine($"    {d:yyyy 年 MM 月 dd 日}（{d.ToNongliLunarString()}）");
    }
    Console.Write($"在确认后请按回车继续：");
    line = Console.ReadLine();
    if (line is null)
        return;

    Console.WriteLine();
}
#endregion

#region 三、创建草稿
{
    Console.WriteLine("三、创建草稿");

    Console.WriteLine($"正在加载卦爻辞……");
    ZhouyiStoreWithLineTitles zhouyi;
    {
        var storeContent = File.ReadAllText("./zhouyi.json");
        var zhouyiStore = ZhouyiStore.DeserializeFromJsonString(storeContent);
        zhouyi = new ZhouyiStoreWithLineTitles(zhouyiStore!);
    }

    foreach (var (date, i) in dates.Select((d, i) => (d, i + 1)))
    {
        Console.WriteLine($"正在为" +
            $"{date: yyyy 年 MM 月 dd 日}（{date.ToNongliLunarString()}）" +
            $"创建草稿（ {i}/{dates.Count} ）……");

        var gua = new HexagramProvider(date).GetHexagram();
        var hexagram = zhouyi[gua];
        var (upper, lower) = hexagram.SplitToTrigrams(zhouyi.InnerStore);

        string guaTitle;
        if (upper.Painting == lower.Painting)
            guaTitle = $"{hexagram.Name}为{upper.Nature}";
        else
            guaTitle = $"{upper.Nature}{lower.Nature}{hexagram.Name}";

        var contentBuilder = new StringBuilder();
        _ = contentBuilder.Append("<div style=\"line-height: 1.75em;\">");
        {
            var s = hexagram.Text?.Split('：');

            _ = contentBuilder.Append("<strong>");
            _ = contentBuilder.Append(s?[0]);
            _ = contentBuilder.Append("</strong>");
            _ = contentBuilder.Append('：');
            _ = contentBuilder.Append(s?[1]);
            _ = contentBuilder.Append("<br>");

            _ = contentBuilder.Append(hexagram.Xiang);
            _ = contentBuilder.Append("<br>");

            _ = contentBuilder.Append(hexagram.Tuan);
            _ = contentBuilder.Append("<br>");
            _ = contentBuilder.Append("<br>");
        }

        foreach (var line in hexagram.EnumerateYaos())
        {
            if (line.YaoText is not null)
            {
                var s = line.YaoText.Split('：');

                _ = contentBuilder.Append("<strong>");
                _ = contentBuilder.Append(s?[0]);
                _ = contentBuilder.Append("</strong>");
                _ = contentBuilder.Append('：');
                _ = contentBuilder.Append(s?[1]);
                _ = contentBuilder.Append("<br>");

                _ = contentBuilder.Append(line.Xiang);
                _ = contentBuilder.Append("<br>");
            }
        }
        _ = contentBuilder.Append("</div>");

        var request = new
        {
            articles = new[] {
                new {
                    title = $"{gua.ToUnicodeChar()} {guaTitle} {date.ToNongliLunarString()}",
                    // author = "author",
                    digest = hexagram.Text?.Split('：')[1],
                    content = contentBuilder.ToString(),
                    content_source_url = $"https://onehexagramperday.yueyinqiu.top/?p={gua}",
                    thumb_media_id = "ZVlt-CLS2_hum7At1kporE6viau0_PbRNQdcD9vopWxuxGwtZskatwe07c2JOzrQ",
                    // need_open_comment = 0,
                    // only_fans_can_comment = 0,
                    // pic_crop_235_1 = "X1_Y1_X2_Y2",
                    // pic_crop_1_1 = "X1_Y1_X2_Y2",
                }
            }
        };

        var response = await client.PostAsWeChatThenFromJson<dynamic, JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/draft/add?" +
            $"access_token={accessToken}",
            request);
        if (response?["media_id"] is null)
        {
            Console.WriteLine("草稿创建失败，具体响应为：");
            Console.WriteLine(response);
            Console.WriteLine("发送的信息为：");
            Console.WriteLine(request);
            return;
        }
    }
    Console.WriteLine($"已全部完成。");
    Console.WriteLine();
}
#endregion
