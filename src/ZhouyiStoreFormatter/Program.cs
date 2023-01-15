using System.Diagnostics;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Core;

var s = File.ReadAllText("./input.json");
var store = ZhouyiStore.DeserializeFromJsonString(s);
Debug.Assert(store is not null);

File.WriteAllText("zhouyi.json", store.SerializeToJsonString());

var yinLineTitles = new[] { "初六：", "六二：", "六三：", "六四：", "六五：", "上六：" };
var yangLineTitles = new[] { "初九：", "九二：", "九三：", "九四：", "九五：", "上九：" };

for (int i = 0; i < 64; i++)
{
    var painting = Painting.Parse(Convert.ToString(i, 2).PadLeft(6, '0'));
    var hexagram = store.GetHexagram(painting);

    var lines = hexagram.EnumerateLines(false).ToArray();
    for (int j = 0; j < 6; j++)
    {
        if (lines[j].YinYang == YinYang.Yang)
        {
            lines[j].LineText = yangLineTitles[j] + lines[j].LineText;
        }
        else
        {
            lines[j].LineText = yinLineTitles[j] + lines[j].LineText;
        }
    }

    if (hexagram.Index == "1")
    {
        hexagram.Yong.LineText = "用九：" + hexagram.Yong.LineText;
    }
    else if (hexagram.Index == "0")
    {
        hexagram.Yong.LineText = "用六：" + hexagram.Yong.LineText;
    }
    else
    {
        hexagram.Yong.LineText = null;
    }

    store.UpdateStore(hexagram);
}

File.WriteAllText("zhouyi-WeChatTextGenerator.json", store.SerializeToJsonString(new() {
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
}));
