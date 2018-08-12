using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorStorage
    {
        void OpenCategory(ref Guid rguidCategory, StorageFlags flags);

        void CloseCategory();

        void RemoveCategory(ref Guid rguidCategory);

        bool GetFont(Logfont[] pLogfont, FontInfo[] pInfo);

        bool GetItem(string name, ColorableItemInfo[] pInfo);

        void SetFont(FontInfo[] pInfo);

        void SetItem(string szName, ColorableItemInfo[] pInfo);

        bool GetItemCount(out int count);

        bool GetItemNameAtIndex(int index, out string itemName);
    }

    [Export(typeof(IFontAndColorStorage))]
    internal class FontAndColorStorage : IFontAndColorStorage
    {
        private readonly IFontAndColorDefaultsProvider _provider;
        private Guid _openGuid = Guid.Empty;

        [ImportingConstructor]
        public FontAndColorStorage(IFontAndColorDefaultsProvider provider)
        {
            _provider = provider;
        }

        public void OpenCategory(ref Guid rguidCategory, StorageFlags flags)
        {
            _openGuid = rguidCategory;
        }

        public void CloseCategory()
        {
            _openGuid = Guid.Empty;
        }

        public void RemoveCategory(ref Guid rguidCategory)
        {
        }

        public bool GetFont(Logfont[] pLogfont, FontInfo[] pInfo)
        {
            if (!(_provider.GetObject(ref _openGuid) is IFontAndColorDefaults store))
                return false;
            return store.GetFont(pInfo) == 0;
        }

        public bool GetItem(string name, ColorableItemInfo[] pInfo)
        {
            if (!(_provider.GetObject(ref _openGuid) is IFontAndColorDefaults store))
                return false;
            if (store.GetItemByName(name, out var item) != 0)
                return false;
            pInfo[0] = item.Info;
            return true;
        }

        public void SetFont(FontInfo[] pInfo)
        {
        }

        public void SetItem(string szName, ColorableItemInfo[] pInfo)
        {
        }

        public bool GetItemCount(out int count)
        {
            count = default;
            if (!(_provider.GetObject(ref _openGuid) is IFontAndColorDefaults store))
                return false;
            count = store.GetItemCount();
            return true;
        }

        public bool GetItemNameAtIndex(int index, out string itemName)
        {
            itemName = default;
            if (!(_provider.GetObject(ref _openGuid) is IFontAndColorDefaults store))
                return false;
            itemName = store.GetItem(index).Name;
            return true;
        }
    }

    [Flags]
    public enum StorageFlags
    {
        Readonly = 1,
        Loaddefaults = 2,
        Propagatechanges = 4,
        Noautocolors = 8,
    }
}