using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.EditorBase.Settings.FontsAndColors
{
    [Export]
    internal class FontsAndColorsSettingsFactory : IPartImportsSatisfiedNotification
    {
        [ImportMany] private IEnumerable<IFontAndColorGroup> _groups;
        [ImportMany] private IEnumerable<IFontAndColorDefaults> _pages;

        private Dictionary<Guid, IFontAndColorDefaultsProvider> _providerMapping = new Dictionary<Guid, IFontAndColorDefaultsProvider>();

        [Import] internal IFontAndColorStorage Storage;
        [ImportMany] internal IEnumerable<IFontAndColorDefaultsProvider> DefaultsProviders;

        private IEnumerable<Guid> _sortedGuids;


        public void OnImportsSatisfied()
        {    
            var sortedGuidMap = new List<KeyValuePair<Guid, int>>();
            foreach (var group in _groups.OrderBy(x => x.GetPriority()))
            {
                var newPair = new KeyValuePair<Guid, int>(group.GroupGuid, group.GetPriority());
                if (!sortedGuidMap.Contains(newPair))
                    sortedGuidMap.Add(newPair);
            }

            foreach (var page in _pages.OrderBy(x => x.GetPriority()))
            {
                var newPair = new KeyValuePair<Guid, int>(page.CategoryGuid, page.GetPriority());
                if (sortedGuidMap.Contains(newPair))
                    continue;

                var index = GetInsertIndex(newPair.Key, newPair.Value, in sortedGuidMap);
                sortedGuidMap.Insert(index, newPair);
            }
            _sortedGuids = sortedGuidMap.Select(x => x.Key);
        }

        public IEnumerable<FontNameItem> GetFonNames()
        {
            var monoSpaceFonts = InstalledFontHelper.GetInstalledMonospaceFonts();
            var allFonts = InstalledFontHelper.GetInstalledFonts();

            var monospaceItems = monoSpaceFonts.Select(x => new FontNameItem(x.Name, true));
            var remaining = allFonts.Except(monoSpaceFonts).Select(x => new FontNameItem(x.Name, false));
            return monospaceItems.Union(remaining).Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Name);
        }

        public IEnumerable<FontColorCategoryItem> GetCategories()
        {
            foreach (var guid in _sortedGuids)
            {
                var group = _groups.FirstOrDefault(x => x.GroupGuid == guid);
                if (group != null)
                {
                    yield return new FontColorCategoryItem(guid, group.GetGroupName(), true);
                }
                else
                {
                    var page = _pages.FirstOrDefault(x => x.CategoryGuid == guid);
                    if (page != null)
                    {
                        yield return new FontColorCategoryItem(guid, page.GetCategoryName(), false);
                    }
                }
            }
        }

        public IEnumerable<FontColorEntry> ItemEntriesFromCategory(Guid categoryId)
        {
            var provider = GetProvider(categoryId);
            if (provider == null)
                return new List<FontColorEntry>();

            var category = provider.GetObject(categoryId);
            if (category is IFontAndColorDefaults defaults)
                return ItemEntriesFromDefaults(defaults);
            if (category is IFontAndColorGroup group)
                return ItemEntriesFromGroup(group);

            return new List<FontColorEntry>();
        }

        private static IEnumerable<FontColorEntry> ItemEntriesFromDefaults(IFontAndColorDefaults defaults)
        {
            for (var i = 0; i < defaults.GetItemCount(); ++i)
            {
                yield return new FontColorEntry(defaults.GetItem(i).Name);
            }
        }

        private IEnumerable<FontColorEntry> ItemEntriesFromGroup(IFontAndColorGroup group)
        {
            var hashSet = new HashSet<FontColorEntry>();

            for (var i = 0; i < group.GetCount(); ++i)
            {
                var list = ItemEntriesFromCategory(group.GetCategory(i));
                foreach (var entry in list.ToList())
                {
                    if (!hashSet.Contains(entry))
                        hashSet.Add(entry);
                }
            }

            return hashSet;
        }

        internal IFontAndColorDefaultsProvider GetProvider(Guid category)
        {
            if (!_providerMapping.TryGetValue(category, out var provider))
            {
                provider = DefaultsProviders.FirstOrDefault(x => x.GetObject(category) != null);
                if (provider != null)
                    _providerMapping.Add(category, provider);
            }
            return provider;
        }

        private int GetInsertIndex(Guid pageId, int priority, in List<KeyValuePair<Guid, int>> currentList)
        {

            var parent = Guid.Empty;
            foreach (var group in _groups)
            {
                if (group.ContainsCategory(pageId))
                {
                    parent = group.GroupGuid;
                    break;
                }
            }

            int index;
            if (parent == Guid.Empty)
                index = currentList.IndexOf(currentList.FirstOrDefault(x => x.Value <= priority)) + 1;
            else
                index = currentList.IndexOf(currentList.LastOrDefault(x => x.Value <= priority)) + 1;



            return index;
        }
    }

    internal static class FontAndColorGroupExtensions
    {
        internal static bool ContainsCategory(this IFontAndColorGroup group, Guid category)
        {
            for (var i = 0; i < group.GetCount(); ++i)
            {
                if (group.GetCategory(i) == category)
                    return true;
            }
            return false;
        }
    }
}