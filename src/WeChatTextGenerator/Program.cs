using com.nlf.calendar;
using System.Diagnostics;
using System.Text;
using YiJingFramework.Core;
using YiJingFramework.Painting.Presenting;
using YiJingFramework.References.Zhouyi;
using YiJingFramework.References.Zhouyi.Zhuan;

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

Console.OutputEncoding = Encoding.UTF8;

new Program(date).Run();

internal partial class Program
{
    private readonly DateOnly date;
    private readonly Painting hexagram;

    private readonly Zhouyi jing;
    private readonly Tuanzhuan tuan;
    private readonly XiangZhuan xiang;

    private static readonly CharacterConverter characterConverter = new CharacterConverter();

    internal Program(DateOnly date)
    {
        this.date = date;
        this.hexagram = HexagramProvider.Provider.GetHexagram(date);

        using FileStream jingFile = new FileStream("./jing.json", FileMode.Open, FileAccess.Read);
        this.jing = new Zhouyi(jingFile);
        using FileStream tuanFile = new FileStream("./tuan.json", FileMode.Open, FileAccess.Read);
        this.tuan = new Tuanzhuan(tuanFile);
        using FileStream xiangFile = new FileStream("./xiang.json", FileMode.Open, FileAccess.Read);
        this.xiang = new XiangZhuan(xiangFile);
    }

    private void Print(Painting hexagramPainting)
    {
        IEnumerable<ZhouyiHexagram.Line> AsEnumerable(ZhouyiHexagram zhouyiHexagram)
        {
            yield return zhouyiHexagram.FirstLine;
            yield return zhouyiHexagram.SecondLine;
            yield return zhouyiHexagram.ThirdLine;
            yield return zhouyiHexagram.FourthLine;
            yield return zhouyiHexagram.FifthLine;
            yield return zhouyiHexagram.SixthLine;
        }
        string lunarStr;
        {
            var lunar = Lunar.fromDate(this.date.ToDateTime(new TimeOnly(6, 30)));
            lunarStr = $"{lunar.getYearInGanZhi()}年{lunar.getMonthInChinese()}月{lunar.getDayInChinese()}";
        }

        Console.WriteLine("====日期================");
        Console.WriteLine($"{this.date:yyyy/MM/dd}");
        Console.WriteLine(lunarStr);
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        Debug.Assert(hexagramPainting.Count is 6);
        ZhouyiHexagram hexagram = this.jing.GetHexagram(hexagramPainting);

        ZhouyiTrigram upper = hexagram.UpperTrigram;
        ZhouyiTrigram lower = hexagram.LowerTrigram;

        Console.WriteLine("====标题================");
        Console.Write(characterConverter.ConvertTo(hexagramPainting));
        if (upper == lower)
            Console.WriteLine($" {hexagram.Name}为{upper.Nature} {lunarStr}");
        else
            Console.WriteLine($" {upper.Nature}{lower.Nature}{hexagram.Name} {lunarStr}");
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("====正文================");
        Console.WriteLine($"{hexagram.Name}：{hexagram.Text}");
        Console.WriteLine($"象曰：{this.xiang[hexagram]}");
        Console.WriteLine($"彖曰：{this.tuan[hexagram]}");
        Console.WriteLine();

        foreach (var line in AsEnumerable(hexagram))
        {
            Console.WriteLine(line);
            Console.WriteLine($"象曰：{this.xiang[line]}");
        }

        var applyNinesOrApplySixes = hexagram.ApplyNinesOrApplySixes;
        if (applyNinesOrApplySixes is not null)
        {
            Console.WriteLine(applyNinesOrApplySixes.ToString().TrimEnd());
            Console.WriteLine($"象曰：{this.xiang[applyNinesOrApplySixes]}");
        }
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("====网址================");
        Console.WriteLine($"https://onehexagramperday.nololiyt.top/query?p={hexagramPainting}");
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine();
    }

    internal void Run()
    {
        this.Print(this.hexagram);
    }
}