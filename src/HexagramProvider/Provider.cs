using YiJingFramework.Core;

namespace HexagramProvider
{
    public static class Provider
    {
        private static IEnumerable<YinYang> RandomYinYangs(int seed)
        {
            Random random = new Random(seed);
            for (; ; )
                yield return (YinYang)random.Next(0, 2);
        }

        public static Painting GetHexagram(DateOnly date)
        {
            var seed = date.DayNumber;
            return new Painting(RandomYinYangs(seed).Take(6));
        }
    }
}