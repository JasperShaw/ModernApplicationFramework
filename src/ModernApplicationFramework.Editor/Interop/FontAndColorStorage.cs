using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.Interop
{
    [Export(typeof(IFontAndColorStorage))]
    internal class FontAndColorStorage : IFontAndColorStorage
    {
        private readonly IEnumerable<IFontAndColorDefaultsProvider> _providers;
        private Guid _openGuid = Guid.Empty;

        private IFontAndColorDefaultsProvider CurrentProvider { get; set; }

        [ImportingConstructor]
        public FontAndColorStorage([ImportMany] IEnumerable<IFontAndColorDefaultsProvider> providers)
        {
            _providers = providers;
        }

        public void OpenCategory(Guid rguidCategory, StorageFlags flags)
        {
            var provider = _providers.FirstOrDefault(x => x.GetObject(rguidCategory) != null);
            _openGuid = rguidCategory;
            CurrentProvider = provider ?? throw new ArgumentException("Could not find any category");
        }

        public void CloseCategory()
        {
            CurrentProvider = null;
            _openGuid = Guid.Empty;
        }

        public void RemoveCategory(Guid rguidCategory)
        {
            throw new NotImplementedException();
        }

        public bool GetFont(Logfont[] pLogfont, FontInfo[] pInfo)
        {
            if (CurrentProvider == null)
                throw new InvalidOperationException("No Category open");
            var obj = CurrentProvider.GetObject(_openGuid);
            if (obj is IFontAndColorDefaults store)
                return store.GetFont(pInfo) == 0;
            if (obj is IFontAndColorGroup group)
            {
                var fontInfos = GetGroupFonts(group).ToList();
                var toCheck = fontInfos.First();
                foreach (var fontInfo in fontInfos)
                {
                    if (!toCheck.Equals(fontInfo))
                        return false;
                }
                return ((IFontAndColorDefaults) CurrentProvider.GetObject(group.GetCategory(0)))?.GetFont(pInfo) == 0;
            }

            return false;
        }

        private IEnumerable<FontInfo> GetGroupFonts(IFontAndColorGroup group)
        {
            var result = new FontInfo[group.GetCount()];
            for (var i = 0; i < group.GetCount(); ++i)
            {
                var t = CurrentProvider.GetObject(group.GetCategory(i));
                if (!(t is IFontAndColorDefaults defaults))
                {
                    continue;
                }

                var field = new[]
                {
                    new FontInfo()
                };
                defaults.GetFont(field);
                result[i] = field[0];
            }
            return result;
        }

        public bool GetItem(string name, ColorableItemInfo[] pInfo)
        {
            if (CurrentProvider == null)
                throw new InvalidOperationException("No Category open");
            if (CurrentProvider.GetObject(_openGuid) is IFontAndColorDefaults store)
            {
                if (store.GetItemByName(name, out var item) != 0)
                    return false;
                pInfo[0] = item.Info;
                return true;
            }

            return false;

        }

        public void SetFont(FontInfo[] pInfo)
        {
            throw new NotImplementedException();
        }

        public void SetItem(string szName, ColorableItemInfo[] pInfo)
        {
            throw new NotImplementedException();
        }

        public bool GetItemCount(out int count)
        {
            count = default;
            if (CurrentProvider == null)
                throw new InvalidOperationException("No Category open");
            if (!(CurrentProvider.GetObject(_openGuid) is IFontAndColorDefaults store))
                return false;
            count = store.GetItemCount();
            return true;
        }

        public bool GetItemNameAtIndex(int index, out string itemName)
        {
            itemName = default;
            if (CurrentProvider == null)
                throw new InvalidOperationException("No Category open");
            if (!(CurrentProvider.GetObject(_openGuid) is IFontAndColorDefaults store))
                return false;
            itemName = store.GetItem(index).Name;
            return true;
        }
    }
}