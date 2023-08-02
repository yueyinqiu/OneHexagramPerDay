using System.Diagnostics;
using System.Text.Json;
using YiJingFramework.Annotating.Zhouyi;

var storeLink = "https://yueyinqiu.github.io/my-yijing-annotation-stores/975345ca/2023-08-02-1.json";
using var httpClient = new HttpClient();
var s = await httpClient.GetStringAsync(storeLink);
var store = ZhouyiStore.DeserializeFromJsonString(s);
Debug.Assert(store is not null);

store.Tags.Clear();
store.Tags.Add(
    $"This store file was edited from `{storeLink}` for OneHexagramPerDay's use. " +
    $"Some unused entries have been removed.");

store.Title = $"{store.Title} For OneHexagramPerDay";

var xugua = store.GetXugua();
xugua.Content = null;
store.UpdateStore(xugua);

var zagua = store.GetZagua();
zagua.Content = null;
store.UpdateStore(zagua);

var xici = store.GetXici();
xici.PartA = null;
xici.PartB = null;
store.UpdateStore(xici);

var shuogua = store.GetShuogua();
shuogua.Content = null;
store.UpdateStore(shuogua);

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
