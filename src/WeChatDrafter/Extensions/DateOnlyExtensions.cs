using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;

namespace WeChatDrafter.Extensions;
internal static class DateOnlyExtensions
{
    private static readonly TimeOnly twelve = new TimeOnly(12, 0, 0);
    public static string ToNongliLunarString(this DateOnly dateOnly)
    {
        var dateTime = dateOnly.ToDateTime(twelve);
        var nongli = LunarDateTime.FromGregorian(dateTime);
        return $"{nongli.Nian:C}年{nongli.YueInChinese()}月{nongli.RiInChinese()}";
    }
}
