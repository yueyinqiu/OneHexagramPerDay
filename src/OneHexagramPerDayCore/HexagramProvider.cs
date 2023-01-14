using YiJingFramework.Core;

namespace OneHexagramPerDayCore
{
    public sealed class HexagramProvider
    {
        private HexagramProvider() { }

        public static HexagramProvider Default { get; } = new HexagramProvider();

        private static IEnumerable<YinYang> RandomYinYangs(int seed)
        {
            Random random = new Random(seed);
            for (; ; )
                yield return (YinYang)random.Next(0, 2);
        }

        public Painting GetHexagram(DateOnly date)
        {
            var seed = date.DayNumber;
            return new Painting(RandomYinYangs(seed).Take(6));
        }
    }
}