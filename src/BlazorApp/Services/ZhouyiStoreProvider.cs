using BlazorApp.Serialization;
using OneHexagramPerDayCore;
using System.Net.Http.Json;

namespace BlazorApp.Services
{
    public sealed class ZhouyiStoreProvider
    {
        private readonly Task<ZhouyiStoreWithLineTitles> getStoreTask;

        private static async Task<ZhouyiStoreWithLineTitles> RunGetZhouyiStoreTask(Uri baseUri)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = baseUri
            };
            var store = await httpClient.GetFromJsonAsync(
                "zhouyi.json",
                ZhouyiStoreSerializerContext.Default.ZhouyiStore);
            return store is null ?
                throw new InvalidDataException("Failed to load the zhouyi store.") :
                new(store);
        }

        public ZhouyiStoreProvider(string baseAddress)
        {
            this.getStoreTask = RunGetZhouyiStoreTask(new(baseAddress));
        }

        public async Task<ZhouyiStoreWithLineTitles> GetZhouyiStoreAsync()
        {
            return await this.getStoreTask;
        }
    }
}
