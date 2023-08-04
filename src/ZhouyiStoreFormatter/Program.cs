using System.Diagnostics;
using System.Text.Json;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;

var storeLink = "https://yueyinqiu.github.io/my-yijing-annotation-stores/975345ca/2023-08-02-1.json";
using var httpClient = new HttpClient();
var s = await httpClient.GetStringAsync(storeLink);
var store = ZhouyiStore.DeserializeFromJsonString(s);
Debug.Assert(store is not null);

store.Title = $"{store.Title} For OneHexagramPerDay";

store.Tags.Clear();
store.Tags.Add(
    $"此注解仓库专门为 OneHexagramPerDay 进行过调整，可能缺少部分信息。" +
    $"原仓库：{storeLink}");

store.UpdateStore(new Shuogua());
store.UpdateStore(new Xugua());
store.UpdateStore(new Zagua());
store.UpdateStore(new Xici());

Directory.CreateDirectory("./For BlazorApp/data");
var storeName = $"data/zhouyi-{DateTime.Now:yyyy-MM-dd}.json";
await File.WriteAllTextAsync(
    $"./For BlazorApp/{storeName}",
    store.SerializeToJsonString());
await File.WriteAllTextAsync(
    $"./For BlazorApp/data/zhouyi-location.json",
    JsonSerializer.Serialize(storeName));


Directory.CreateDirectory("./For WeChatTextGenerator");
await File.WriteAllTextAsync(
    "./For WeChatTextGenerator/zhouyi.json",
    store.SerializeToJsonString(new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    }));
