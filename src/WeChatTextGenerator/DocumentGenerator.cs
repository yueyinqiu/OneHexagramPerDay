using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OneHexagramPerDayCore;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

namespace WeChatTextGenerator;

internal partial class DocumentGenerator
{
    private readonly ZhouyiStoreWithLineTitles zhouyi;
    private readonly DirectoryInfo directory;

    internal DocumentGenerator(DirectoryInfo directory, ZhouyiStore zhouyi)
    {
        this.directory = directory;
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

    public void GenerateFor(GuaHexagram gua, Lunar.Lunar lunar)
    {
        var lunarString = $"{lunar.YearInGanZhi}年{lunar.MonthInChinese}月{lunar.DayInChinese}";

        ZhouyiHexagram hexagram = this.zhouyi[gua];
        var (upperPainting, lowerPainting) = hexagram.SplitToTrigrams();
        ZhouyiTrigram upper = this.zhouyi.InnerStore.GetTrigram(upperPainting);
        ZhouyiTrigram lower = this.zhouyi.InnerStore.GetTrigram(lowerPainting);

        var hexagramChar = gua.ToUnicodeChar();
        string title;
        if (upperPainting == lowerPainting)
            title = $"{hexagram.Name}为{upper.Nature} {lunarString}";
        else
            title = $"{upper.Nature}{lower.Nature}{hexagram.Name} {lunarString}";
        var fileName = Path.GetFullPath($"{hexagramChar} {title}.docx", this.directory.FullName);

        this.directory.Create();
        using var word = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document);

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

        word.Save();
    }
}