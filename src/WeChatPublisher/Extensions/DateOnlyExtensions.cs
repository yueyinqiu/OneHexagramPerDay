using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;

namespace WeChatPublisher.Extensions;
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
