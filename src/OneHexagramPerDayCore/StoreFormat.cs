/*
using YiJingFramework.Annotating;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Core;

namespace OneHexagramPerDayCore
{
    public sealed class StoreFormat
    {
        private StoreFormat() { }
        private static AnnotationGroup<string> GetOrNewStringGroup(
            AnnotationStore store,
            string title, ref bool hasChanged)
        {
            var group = store.StringGroups.FirstOrDefault(s => s.Title == title);
            if (group is null)
            {
                group = store.AddStringGroup(title);
                hasChanged = true;
            }
            return group;
        }

        private static AnnotationEntry<string> GetOrNewEntry(
            AnnotationGroup<string> group,
            string target, string defaultValue, ref bool hasChanged)
        {
            var entry = group.Entries.FirstOrDefault(s => s.Target == target);
            if (entry is null)
            {
                entry = group.AddEntry(target, defaultValue);
                hasChanged = true;
            }
            return entry;
        }

        public static void EnsureFormattingGroup(ZhouyiStore store, out bool hasChanged)
        {
            hasChanged = false;
            var group = GetOrNewStringGroup(store.Store,
                "OneHexagramPerDay.Formatting", ref hasChanged);

            _ = GetOrNewEntry(group, "Format.LineText.Yin.1", "初六：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.2", "六二：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.3", "六三：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.4", "六四：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.5", "六五：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.6", "上六：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yin.0", "用六：{0}", ref hasChanged);

            _ = GetOrNewEntry(group, "Format.LineText.Yang.1", "初九：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.2", "九二：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.3", "九三：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.4", "九四：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.5", "九五：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.6", "上九：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.LineText.Yang.0", "用九：{0}", ref hasChanged);

            _ = GetOrNewEntry(group, "Format.LineXiang", "象曰：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.HexagramXiang", "象曰：{0}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.Tuan", "彖曰：{0}", ref hasChanged);

            _ = GetOrNewEntry(group, "Format.HexagramText", "{0}：{1}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.HexagramTitle.SameUpperLower", "{0}为{1}", ref hasChanged);
            _ = GetOrNewEntry(group, "Format.HexagramTitle.DifferentUpperLower", "{0}{1}{2}", ref hasChanged);
        }

        private static string GetEntryContentOrDefault(
            AnnotationGroup<string>? group,
            string target, string defaultValue)
        {
            return group?.Entries?.FirstOrDefault(s => s.Target == target)?.Content ?? defaultValue;
        }
        
        
        public static StoreFormat CreateFormat(ZhouyiStore store)
        {
            var group = store.Store.StringGroups
                .FirstOrDefault(s => s.Title == "OneHexagramPerDay.Formatting");

            var yinLineFormat = new string[7] {
                GetEntryContentOrDefault(group, "Format.LineText.Yin.1", "初六：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.2", "六二：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.3", "六三：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.4", "六四：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.5", "六五：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.6", "上六：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yin.0", "用六：{0}")
            };

            var yangLineFormat = new string[7] {
                GetEntryContentOrDefault(group, "Format.LineText.Yang.1", "初九：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.2", "九二：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.3", "九三：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.4", "九四：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.5", "九五：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.6", "上九：{0}"),
                GetEntryContentOrDefault(group, "Format.LineText.Yang.0", "用九：{0}")
            };

            var lineXiangFormat = GetEntryContentOrDefault(
                group, "Format.LineXiang", "象曰：{0}");
            var hexagramXiangFormat = GetEntryContentOrDefault(
                group, "Format.HexagramXiang", "象曰：{0}");
            var tuanFormat = GetEntryContentOrDefault(
                group, "Format.Tuan", "彖曰：{0}");

            var hexagramTextFormat = GetEntryContentOrDefault(
                group, "Format.HexagramText", "{0}：{1}");
            var hexagramTitleFormatSame = GetEntryContentOrDefault(
                group, "Format.HexagramTitle.SameUpperLower", "{0}为{1}");
            var hexagramTitleFormatDifferent = GetEntryContentOrDefault(
                group, "Format.HexagramTitle.DifferentUpperLower", "{0}{1}{2}");
        }
    }
}
*/