using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OneHexagramPerDayCore;
using System.Diagnostics;
using System.Text;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelationships.MostAccepted.GuaToCharacterExtensions;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

var current = DateTime.Now;

static DateOnly ParseToDateTime(string s, string? format = null)
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

    private readonly ZhouyiStoreWithLineTitles zhouyi;

    internal Program(DateOnly date)
    {
        this.date = date;
        this.hexagramPainting = new HexagramProvider(date).GetHexagram();

        var storeContent = File.ReadAllText("./zhouyi.json");
        var zhouyi = ZhouyiStore.DeserializeFromJsonString(storeContent);
        Debug.Assert(zhouyi is not null);
        this.zhouyi = new(zhouyi);
    }

    private static void AppendLine(Body body, string? text = null)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());
        var properties = run.AppendChild(new RunProperties());
        _ = properties.AppendChild(new FontSize()
        {
            Val = "26"
        });
        if (text is not null)
            _ = run.AppendChild(new Text(text));
    }

    internal void Run()
    {
        var lunar = Lunar.Lunar.FromDate(this.date.ToDateTime(new TimeOnly(6, 30)));
        var lunarStr = $"{lunar.YearInGanZhi}年{lunar.MonthInChinese}月{lunar.DayInChinese}";
        Console.WriteLine($"{this.date:yyyy/MM/dd} {lunarStr}");

        ZhouyiHexagram hexagram = this.zhouyi[this.hexagramPainting];
        var (upperPainting, lowerPainting) = hexagram.SplitToTrigrams();
        ZhouyiTrigram upper = this.zhouyi.InnerStore.GetTrigram(upperPainting);
        ZhouyiTrigram lower = this.zhouyi.InnerStore.GetTrigram(lowerPainting);

        var hexagramChar = this.hexagramPainting.ToUnicodeChar();
        string title;
        if (upperPainting == lowerPainting)
            title = $"{hexagram.Name}为{upper.Nature} {lunarStr}";
        else
            title = $"{upper.Nature}{lower.Nature}{hexagram.Name} {lunarStr}";

        var wordFileInfo = new FileInfo($"./out/{hexagramChar} {title}.docx");
        wordFileInfo.Directory?.Create();
        using var word = WordprocessingDocument.Create(
            wordFileInfo.FullName, WordprocessingDocumentType.Document);
        var document = new Document();
        word.AddMainDocumentPart().Document = document;
        var body = document.AppendChild(new Body());

        AppendLine(body, hexagram.Text);
        AppendLine(body, hexagram.Xiang);
        AppendLine(body, hexagram.Tuan);
        AppendLine(body);
        foreach (var line in hexagram.EnumerateLines())
        {
            if (line.LineText is not null)
            {
                AppendLine(body, line.LineText);
                AppendLine(body, line.Xiang);
            }
        }
        AppendLine(body, $"https://onehexagramperday.nololiyt.top/?p={this.hexagramPainting}");

        Console.WriteLine("完成");
    }
}