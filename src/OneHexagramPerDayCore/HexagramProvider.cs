using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

namespace OneHexagramPerDayCore;

public sealed class HexagramProvider
{
    public int Seed { get; }
    public HexagramProvider(int seed)
    {
        this.Seed = seed;
    }
    public HexagramProvider(DateOnly date) : this(date.DayNumber) { }

    private IEnumerable<Yinyang> RandomYinYangs()
    {
        Random random = new Random(this.Seed);
        for (; ; )
            yield return (Yinyang)random.Next(0, 2);
    }

    public GuaHexagram GetHexagram()
    {
        return new GuaHexagram(this.RandomYinYangs().Take(6));
    }
}