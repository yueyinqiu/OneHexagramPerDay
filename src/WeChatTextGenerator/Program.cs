using OneHexagramPerDayCore;
using System.Diagnostics;
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

    outputDirectory.Delete(true);
    var lunar = Lunar.Lunar.FromDate(currentDate.ToDateTime(new TimeOnly(6, 30)));
    generator.GenerateFor(hexagram, lunar);

    ClipboardService.SetText($"https://onehexagramperday.nololiyt.top/?p={hexagram}");

    Console.Write("生成完毕，回车以继续：");
    _ = Console.ReadLine();
    currentDate = currentDate.AddDays(1);
}