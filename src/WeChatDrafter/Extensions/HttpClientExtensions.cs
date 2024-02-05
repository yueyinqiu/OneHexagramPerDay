using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeChatDrafter.Extensions;
internal static class HttpClientExtensions
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static async ValueTask<TResponse?> PostAsWeChatThenFromJson<TRequest, TResponse>(
        this HttpClient client, 
        string url,
        TRequest value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializerOptions);
        var content = new StringContent(json, MediaTypeHeaderValue.Parse("application/json"));
        var response = await client.PostAsync(url, content);
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
}
