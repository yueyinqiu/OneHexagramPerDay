using System.Text.Json.Serialization;
using YiJingFramework.Annotating.Zhouyi;

namespace BlazorApp.Serialization
{
    [JsonSerializable(typeof(ZhouyiStore))]
    public sealed partial class ZhouyiStoreSerializerContext : JsonSerializerContext
    {
    }
}
