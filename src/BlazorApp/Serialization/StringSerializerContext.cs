using System.Text.Json.Serialization;

namespace BlazorApp.Serialization
{
    [JsonSerializable(typeof(string))]
    public sealed partial class StringSerializerContext : JsonSerializerContext
    {
    }
}
