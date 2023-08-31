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

            foreach (var line in hexagram.EnumerateYaos(false))
            {
                Debug.Assert(line.YinYang.HasValue);
                var yinyang = line.YinYang.Value.IsYang ? "九" : "六";
                line.YaoText = line.YaoIndex switch
                {
                    1 => $"初{yinyang}：{line.YaoText}",
                    2 => $"{yinyang}二：{line.YaoText}",
                    3 => $"{yinyang}三：{line.YaoText}",
                    4 => $"{yinyang}四：{line.YaoText}",
                    5 => $"{yinyang}五：{line.YaoText}",
                    _ => $"上{yinyang}：{line.YaoText}",
                };
                line.Xiang = $"象曰：{line.Xiang}";
            }

            var yong = hexagram.Yong;
            if (yong.YaoText is not null)
            {
                string yinyang;
                if (hexagram.Painting == qian)
                    yinyang = "九";
                else if (hexagram.Painting == kun)
                    yinyang = "六";
                else
                    yinyang = "";

                yong.YaoText = $"用{yinyang}：{yong.YaoText}";
                yong.Xiang = $"象曰：{yong.Xiang}";
            }

            var wenyan = hexagram.Wenyan;
            if (hexagram.Wenyan is not null)
                hexagram.Wenyan = $"文言：{wenyan}";

            return hexagram;
        }
    }
}