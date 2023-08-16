using ChineseLunisolarCalendarYjFwkExtensions.Extensions;
using OneHexagramPerDayCore;
using System.Diagnostics;
using System.Globalization;
using TextCopy;
using WeChatTextGenerator;
using YiJingFramework.Annotating.Zhouyi;

var outputDirectory = new DirectoryInfo("./out");
outputDirectory.Create();
Console.WriteLine(
    $"请注意，在生成后，剪切板的内容会被替换为要使用的原文链接。" +
    $"同时，输出文件夹 {outputDirectory.FullName} 中的内容会被删除。");

var currentDate = DateOnly.FromDateTime(DateTime.Now);
Console.Write($"起始日期（默认为 {currentDate:yyyyMMdd} ）：");
var startingDateInput = Console.ReadLine();
if (!string.IsNullOrWhiteSpace(startingDateInput))
    currentDate = DateOnly.ParseExact(startingDateInput, "yyyyMMdd");

var storeContent = File.ReadAllText("./zhouyi.json");
var zhouyi = ZhouyiStore.DeserializeFromJsonString(storeContent);
Debug.Assert(zhouyi is not null);

DocumentGenerator generator = new DocumentGenerator(outputDirectory, zhouyi);
for (; ; )
{
    var hexagram = new HexagramProvider(currentDate).GetHexagram();

    var dateTime = currentDate.ToDateTime(new TimeOnly(6, 30));
    var calendar = new ChineseLunisolarCalendar();
    var nongliTimeTitle = 
        $"{calendar.GetYearGanzhiInChinese(dateTime)}年" +
        $"{calendar.GetMonthInChinese(dateTime)}月" +
        $"{calendar.GetDayInChinese(dateTime)}";

    Console.WriteLine();
    Console.WriteLine($"正在为{nongliTimeTitle}生成……");

    outputDirectory.Delete(true);
    generator.GenerateFor(hexagram, nongliTimeTitle);
    Console.WriteLine($"文件生成已完成。");

    var link = $"https://onehexagramperday.nololiyt.top/?p={hexagram}";
    ClipboardService.SetText(link);
    Console.WriteLine($"原文链接 {link} 已自动复制。");

    Console.Write("生成完毕。回车以继续下一日：");
    _ = Console.ReadLine();
    currentDate = currentDate.AddDays(1);
}