
using BlazorApp.Serialization;
using System.Diagnostics;
using System.Text.Json;
using YiJingFramework.Annotating.Zhouyi;

namespace BlazorApp.Services
{
    public sealed class ZhouyiStoreProvider
    {
        private readonly Uri baseUri;
        public ZhouyiStoreProvider(string baseAddress)
        {
            baseUri = new Uri(baseAddress);
        }

        private ZhouyiStore? store;

        public async Task<ZhouyiStore> GetZhouyiStoreAsync()
        {
            if (store is null)
            {
                var httpClient = new HttpClient() { BaseAddress = baseUri };
                using (var stream = await httpClient.GetStreamAsync("data/zhouyi.json"))
                {
                    var typeInfo = ZhouyiStoreSerializerContext.Default.ZhouyiStore;
                    var store = JsonSerializer.Deserialize(stream, typeInfo);
                    Debug.Assert(store is not null);
                    this.store = store;
                }
            }
            return store;
        }
    }
}
