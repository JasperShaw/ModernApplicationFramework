using System;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorStorage
    {
        void OpenCategory(Guid rguidCategory, StorageFlags flags);

        void CloseCategory();

        void RemoveCategory(Guid rguidCategory);

        bool GetFont(Logfont[] pLogfont, FontInfo[] pInfo);

        bool GetItem(string name, ColorableItemInfo[] pInfo);

        void SetFont(FontInfo[] pInfo);

        void SetItem(string szName, ColorableItemInfo[] pInfo);

        bool GetItemCount(out int count);

        bool GetItemNameAtIndex(int index, out string itemName);
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