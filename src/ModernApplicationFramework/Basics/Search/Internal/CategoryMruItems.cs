using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class CategoryMruItems
    {
        internal const int DefaultMaxMruItems = 100;
        internal int LastMruIndex = -1;
        internal int MaxMruItems = 100;

        internal List<ItemData> MruItems = new List<ItemData>();

        internal Guid CategoryGuid { get; }

        internal bool IsDirty { get; private set; }

        public CategoryMruItems(Guid categoryGuid)
        {
            CategoryGuid = categoryGuid;
            IsDirty = false;
        }

        public void AddMruItem(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            DeleteMruItem(text);
            AddMruItemCore(text);
        }

        public bool DeleteMruItem(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            for (var i = 0; i < MruItems.Count; ++i)
                if (string.Equals(MruItems[i].Value, text, StringComparison.CurrentCultureIgnoreCase))
                {
                    MruItems.RemoveAt(i);
                    if (LastMruIndex >= i)
                        --LastMruIndex;
                    IsDirty = true;
                    return true;
                }

            return false;
        }

        public uint GetMruItems(string prefix, uint maxResults, string[] strArray)
        {
            if (strArray == null && maxResults != 0)
                throw new ArgumentNullException(nameof(strArray));
            if (strArray != null && strArray.Length < maxResults)
                throw new ArgumentException(nameof(maxResults));
            if (maxResults == 0 && prefix == null)
                return (uint) MruItems.Count;
            uint num = 0;
            var index = LastMruIndex;

            do
            {
                if (index < 0)
                {
                    index = MruItems.Count - 1;
                    if (index <= LastMruIndex)
                        break;
                }

                if (string.IsNullOrEmpty(prefix) ||
                    MruItems[index].Value.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    ++num;
                    if (strArray != null)
                    {
                        strArray[num - 1] = MruItems[index].Value;
                        if (num >= maxResults)
                            break;
                    }
                }

                --index;
            } while (index != LastMruIndex);

            return num;
        }

        public void SetMruItem(string itemText)
        {
            if (!DeleteMruItem(itemText))
                throw new ArgumentException(nameof(itemText));
            AddMruItemCore(itemText);
        }

        private void AddMruItemCore(string text)
        {
            var itemData = new ItemData(-1, text);
            if (MruItems.Count < MaxMruItems)
            {
                ++LastMruIndex;
                MruItems.Insert(LastMruIndex, itemData);
            }
            else
            {
                ++LastMruIndex;
                if (LastMruIndex == MruItems.Count)
                    LastMruIndex = 0;
                MruItems[LastMruIndex] = itemData;
            }

            IsDirty = true;
        }

        internal class ItemData
        {
            public int Index { get; set; }

            public bool IsDirty { get; set; }
            public string Value { get; set; }

            public ItemData(int index, string value)
            {
                Index = index;
                Value = value;
            }
        }
    }
}