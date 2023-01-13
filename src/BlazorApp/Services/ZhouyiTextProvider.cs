using YiJingFramework.References.Zhouyi;
using YiJingFramework.References.Zhouyi.Zhuan;

namespace BlazorApp.Services
{
    public sealed class ZhouyiTextProvider
    {
        private readonly Uri baseUri;
        public ZhouyiTextProvider(string baseAddress)
        {
            this.baseUri = new Uri(baseAddress);
        }

        private Zhouyi? zhouyi;
        private XiangZhuan? xiang;
        private Tuanzhuan? tuan;

        public async Task<Zhouyi> GetJingAsync()
        {
            if (this.zhouyi is null)
            {
                var httpClient = new HttpClient() { BaseAddress = baseUri };
                using (var stream = await httpClient.GetStreamAsync("zhouyi/jing.json"))
                    this.zhouyi = new Zhouyi(stream);
            }
            return this.zhouyi;
        }
        public async Task<XiangZhuan> GetXiangAsync()
        {
            if (this.xiang is null)
            {
                var httpClient = new HttpClient() { BaseAddress = baseUri };
                using (var stream = await httpClient.GetStreamAsync("zhouyi/xiang.json"))
                    this.xiang = new XiangZhuan(stream);
            }
            return this.xiang;
        }
        public async Task<Tuanzhuan> GetTuanAsync()
        {
            if (this.tuan is null)
            {
                var httpClient = new HttpClient() { BaseAddress = baseUri };
                using (var stream = await httpClient.GetStreamAsync("zhouyi/tuan.json"))
                    this.tuan = new Tuanzhuan(stream);
            }
            return this.tuan;
        }


        public async Task<(Zhouyi, XiangZhuan, Tuanzhuan)> GetAllAsync()
        {
            var jing = this.GetJingAsync();
            var xiang = this.GetXiangAsync();
            var tuan = this.GetTuanAsync();

            await Task.WhenAll(jing, xiang, tuan);
            return (jing.Result, xiang.Result, tuan.Result);
        }
    }
}
