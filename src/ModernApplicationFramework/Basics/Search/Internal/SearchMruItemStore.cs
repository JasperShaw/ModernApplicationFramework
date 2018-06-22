using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    [Export(typeof(ISearchMruItemStore))]
    internal class SearchMruItemStore : ISearchMruItemStore
    {
        private readonly HybridDictionary _categoriesMruLists = new HybridDictionary();


        public void AddMruItem(ref Guid category, string text)
        {
            var categoryItems = GetCategoryItems(ref category);
            categoryItems.AddMruItem(text);
        }

        public void DeleteMruItem(ref Guid category, string itemText)
        {
            var categoryItems = GetCategoryItems(ref category);
            categoryItems.DeleteMruItem(itemText);
        }

        public uint GetMruItems(ref Guid categoryGuid, string prefix, uint maxResults, string[] strArray)
        {
            return GetCategoryItems(ref categoryGuid).GetMruItems(prefix, maxResults, strArray);
        }

        public void SetMruItem(ref Guid category, string itemText)
        {
            var categoryItems = GetCategoryItems(ref category);
            categoryItems.SetMruItem(itemText);
        }

        private CategoryMruItems GetCategoryItems(ref Guid category)
        {
            if (!(_categoriesMruLists[category] is CategoryMruItems categoryMruItems))
            {
                categoryMruItems = new CategoryMruItems(category);
                _categoriesMruLists[category] = categoryMruItems;
            }

            return categoryMruItems;
        }
    }
}