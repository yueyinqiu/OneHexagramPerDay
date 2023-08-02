using Microsoft.AspNetCore.Components;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

namespace BlazorApp.Pages;

public partial class Hexagram
{
    [Parameter]
    public GuaHexagram? Painting { get; set; }

    private ZhouyiHexagram? hexagram;

    protected override async Task OnParametersSetAsync()
    {
        var store = await this.ZhouyiStoreProvider.GetZhouyiStoreAsync();

        if (this.Painting is null)
            return;
        this.hexagram = store[this.Painting];
    }
}