using System.Diagnostics;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

namespace OneHexagramPerDayCore;

public sealed class ZhouyiStoreWithLineTitles
{
    public ZhouyiStore InnerStore { get; }
    public ZhouyiStoreWithLineTitles(ZhouyiStore store)
    {
        this.InnerStore = store;
    }

    private static readonly GuaHexagram qian = new(Enumerable.Repeat(Yinyang.Yang, 6));
    private static readonly GuaHexagram kun = new(Enumerable.Repeat(Yinyang.Yin, 6));


    public ZhouyiHexagram this[GuaHexagram gua]
    {
        get
        {
            var hexagram = this.InnerStore.GetHexagram(gua);

            hexagram.Text = $"{hexagram.Name}：{hexagram.Text}";
            hexagram.Xiang = $"象曰：{hexagram.Xiang}";
            hexagram.Tuan = $"彖曰：{hexagram.Tuan}";

            foreach (var line in hexagram.EnumerateLines(false))
            {
                Debug.Assert(line.YinYang.HasValue);
                var yinyang = line.YinYang.Value.IsYang ? "九" : "六";
                line.LineText = line.LineIndex switch
                {
                    1 => $"初{yinyang}：{line.LineText}",
                    2 => $"{yinyang}二：{line.LineText}",
                    3 => $"{yinyang}三：{line.LineText}",
                    4 => $"{yinyang}四：{line.LineText}",
                    5 => $"{yinyang}五：{line.LineText}",
                    _ => $"上{yinyang}：{line.LineText}",
                };
                line.Xiang = $"象曰：{line.Xiang}";
            }

            var yong = hexagram.Yong;
            if (yong.LineText is not null)
            {
                string yinyang;
                if (hexagram.Painting == qian)
                    yinyang = "九";
                else if (hexagram.Painting == kun)
                    yinyang = "六";
                else
                    yinyang = "";

                yong.LineText = $"用{yinyang}：{yong.LineText}";
                yong.Xiang = $"象曰：{yong.Xiang}";
            }

            var wenyan = hexagram.Wenyan;
            if (hexagram.Wenyan is not null)
                hexagram.Wenyan = $"文言：{wenyan}";

            return hexagram;
        }
    }
}