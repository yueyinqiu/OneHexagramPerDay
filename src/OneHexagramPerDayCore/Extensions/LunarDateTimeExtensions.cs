﻿using YiJingFramework.Nongli.Lunar;

namespace OneHexagramPerDayCore.Extensions;
public static class LunarDateTimeExtensions
{
    private static readonly string[] yueStrings = new string[]
    {
        "正",
        "二",
        "三",
        "四",
        "五",
        "六",
        "七",
        "八",
        "九",
        "十",
        "冬",
        "腊"
    };

    private static readonly string[] riStingrs = new string[]
    {
        "初一",
        "初二",
        "初三",
        "初四",
        "初五",
        "初六",
        "初七",
        "初八",
        "初九",
        "初十",
        "十一",
        "十二",
        "十三",
        "十四",
        "十五",
        "十六",
        "十七",
        "十八",
        "十九",
        "二十",
        "廿一",
        "廿二",
        "廿三",
        "廿四",
        "廿五",
        "廿六",
        "廿七",
        "廿八",
        "廿九",
        "三十",
    };

    public static string DateToChinese(this LunarDateTime dateTime)
    {
        return $"{dateTime.Nian.Tiangan:C}{dateTime.Nian.Dizhi:C}年" +
            $"{(dateTime.IsRunyue ? "闰" : "")}{yueStrings[dateTime.Yue - 1]}月" +
            $"{riStingrs[dateTime.Ri - 1]}";
    }
}
