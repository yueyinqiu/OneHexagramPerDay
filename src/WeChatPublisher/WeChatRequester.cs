using OneHexagramPerDayCore;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WeChatPublisher.Extensions;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;

namespace WeChatPublisher;

public sealed class WeChatRequester(string appId, string appSecret) : IDisposable
{
    private readonly HttpClient httpClient = new();

    public void Dispose() => this.httpClient.Dispose();

    public async Task<string> GetTokenAsync(bool force = false)
    {
        var response = await this.httpClient.GetFromJsonAsync<JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/token?" +
            $"grant_type=client_credential&" +
            $"appid={appId}&" +
            $"secret={appSecret}");

        var result = response?["access_token"]?.GetValue<string>();
        if (result is null)
        {
            throw new Exception(
                $"令牌解析失败，具体响应为：{Environment.NewLine}{response}");
        }
        return result;
    }

    public async Task<string> UploadDraft(ZhouyiStoreWithLineTitles zhouyi, DateOnly date)
    {
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
                    need_open_comment = 1,
                    // only_fans_can_comment = 0,
                    // pic_crop_235_1 = "X1_Y1_X2_Y2",
                    // pic_crop_1_1 = "X1_Y1_X2_Y2",
                }
            }
        };

        var accessToken = await this.GetTokenAsync();
        var response = await this.httpClient.PostAsWeChatThenFromJson<dynamic, JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/draft/add?" +
            $"access_token={accessToken}",
            request);
        var mediaId = response?["media_id"]?.GetValue<string>();
        if (mediaId is null)
        {
            throw new Exception(
                $"草稿创建失败，具体响应为：{Environment.NewLine}" +
                $"{response}{Environment.NewLine}" +
                $"发送的信息为：{Environment.NewLine}" +
                $"{JsonSerializer.Serialize(
                    request,
                    HttpClientExtensions.JsonSerializerOptions)}");
        }
        return mediaId;
    }

    public async Task Publish(string draft)
    {
        var request = new
        {
            media_id = draft
        };
        var accessToken = await this.GetTokenAsync();
        var response = await this.httpClient.PostAsWeChatThenFromJson<dynamic, JsonObject>(
            $"https://api.weixin.qq.com/cgi-bin/freepublish/submit?" +
            $"access_token={accessToken}",
            request);
        var publishId = response?["publish_id"]?.GetValue<string>();
        if (publishId is null)
        {
            throw new Exception(
                $"发布失败，具体响应为：{Environment.NewLine}" +
                $"{response}{Environment.NewLine}" +
                $"发送的信息为：{Environment.NewLine}" +
                $"{JsonSerializer.Serialize(
                    request,
                    HttpClientExtensions.JsonSerializerOptions)}");
        }

        for (; ; )
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            var checkRequest = new
            {
                publish_id = draft
            };

            accessToken = await this.GetTokenAsync();
            var checkResponse = await this.httpClient.PostAsWeChatThenFromJson<dynamic, JsonObject>(
                $"https://api.weixin.qq.com/cgi-bin/freepublish/get?" +
                $"access_token={accessToken}",
                request);

            var publishStatus = checkResponse?["publish_status"]?.GetValue<int>();
            if (publishStatus is null)
            {
                throw new Exception(
                    $"发布成功，但发布检查失败，具体响应为：{Environment.NewLine}" +
                    $"{checkResponse}{Environment.NewLine}" +
                    $"发送的信息为：{Environment.NewLine}" +
                    $"{JsonSerializer.Serialize(
                        checkRequest,
                        HttpClientExtensions.JsonSerializerOptions)}{Environment.NewLine}" +
                    $"发布时的响应为：{Environment.NewLine}" +
                    $"{response}{Environment.NewLine}" +
                    $"发送的信息为：{Environment.NewLine}" +
                    $"{JsonSerializer.Serialize(
                        request,
                        HttpClientExtensions.JsonSerializerOptions)}{Environment.NewLine}");
            }

            if (publishStatus == 0)
                return;

            if (publishStatus == 1)
                continue;

            throw new Exception(
                $"发布请求成功，但发布失败，具体响应为：{Environment.NewLine}" +
                $"{checkResponse}{Environment.NewLine}" +
                $"发送的信息为：{Environment.NewLine}" +
                $"{JsonSerializer.Serialize(
                    checkRequest,
                    HttpClientExtensions.JsonSerializerOptions)}{Environment.NewLine}" +
                $"发布时的响应为：{Environment.NewLine}" +
                $"{response}{Environment.NewLine}" +
                $"发送的信息为：{Environment.NewLine}" +
                $"{JsonSerializer.Serialize(
                    request,
                    HttpClientExtensions.JsonSerializerOptions)}{Environment.NewLine}");
        }
    }
}
