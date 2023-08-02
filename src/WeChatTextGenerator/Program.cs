using OneHexagramPerDayCore;
using System.Diagnostics;
using System.Text;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelationships.MostAccepted.GuaToCharacterExtensions;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

var current = DateTime.Now;

DateOnly ParseToDateTime(string s, string? format = null)
{
    format = format ?? "yyyyMMdd";
    return DateOnly.ParseExact(s, format);
}

var date = (args.Length is 0 ? "0" : args[0]) switch
{
    "1" => DateOnly.FromDateTime(current.AddDays(1)),
    "2" => ParseToDateTime(args[1]),
    _ => DateOnly.FromDateTime(current)
};

Console.OutputEncoding = Encoding.Unicode;

new Program(date).Run();

internal partial class Program
{
    private readonly DateOnly date;
    private readonly GuaHexagram hexagramPainting;

    private readonly ZhouyiStore zhouyi;

    internal Program(DateOnly date)
    {
        this.date = date;
        this.hexagramPainting = new HexagramProvider(date).GetHexagram();

        var storeContent = File.ReadAllText("./zhouyi-WeChatTextGenerator.json");
        var zhouyi = ZhouyiStore.DeserializeFromJsonString(storeContent);
        Debug.Assert(zhouyi is not null);
        this.zhouyi = zhouyi;
    }

    internal void Run()
    {
        var lunar = Lunar.Lunar.FromDate(this.date.ToDateTime(new TimeOnly(6, 30)));
        var lunarStr = $"{lunar.YearInGanZhi}年{lunar.MonthInChinese}月{lunar.DayInChinese}";

        Console.WriteLine("====日期================");
        Console.WriteLine($"{this.date:yyyy/MM/dd}");
        Console.WriteLine(lunarStr);
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        ZhouyiHexagram hexagram = this.zhouyi.GetHexagram(this.hexagramPainting);

        var (upperPainting, lowerPainting) = hexagram.SplitToTrigrams();

        ZhouyiTrigram upper = this.zhouyi.GetTrigram(upperPainting);
        ZhouyiTrigram lower = this.zhouyi.GetTrigram(lowerPainting);

        Console.WriteLine("====标题================");
        Console.Write(this.hexagramPainting.ToUnicodeChar());
        if (upperPainting == lowerPainting)
            Console.WriteLine($" {hexagram.Name}為{upper.Nature} {lunarStr}");
        else
            Console.WriteLine($" {upper.Nature}{lower.Nature}{hexagram.Name} {lunarStr}");
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("====正文================");
        Console.WriteLine($"{hexagram.Name}：{hexagram.Text}");
        Console.WriteLine($"象曰：{hexagram.Xiang}");
        Console.WriteLine($"彖曰：{hexagram.Tuan}");
        Console.WriteLine();

        foreach (var line in hexagram.EnumerateLines())
        {
            if (line.LineText is not null)
            {
                Console.WriteLine(line.LineText);
                Console.WriteLine($"象曰：{line.Xiang}");
            }
        }

        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("====网址================");
        Console.WriteLine($"https://onehexagramperday.nololiyt.top/?p={this.hexagramPainting}");
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();
    }
}