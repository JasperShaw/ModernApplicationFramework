using System;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxSearchTask : SearchTask
    {
        private readonly IToolboxPrivate _toolbox;

        internal ToolboxSearchTask(uint cookie, ISearchQuery query, ISearchCallback callback, IToolboxPrivate toolbox) : base(cookie, query, callback)
        {
            _toolbox = toolbox;
        }

        protected override void OnStartSearch()
        {

            var currentLayout = _toolbox.CurrentLayout;

            uint result = 0;
            currentLayout.ForEach(x =>
            {
                result += x.SetItemsVisibleWhere(i => i.IsVisible &&
                    CultureInfo.CurrentCulture.CompareInfo.IndexOf(i.Name, SearchQuery.SearchString, CompareOptions.IgnoreCase) >= 0);
            });

            SearchResults = result;
            base.OnStartSearch();
        }

        protected override void OnStopSearch()
        {
            SearchResults = 0;
        }
    }

    public static class CategoryLinq
    {
        public static uint SetItemsVisibleWhere(this IToolboxCategory category, Func<IToolboxItem, bool> func)
        {
            if (!category.HasItems)
                return 0;

            uint count = 0;
            foreach (var categoryItem in category.Items)
            {
                if (!func(categoryItem))
                {
                    categoryItem.IsEnabled = false;
                    categoryItem.IsVisible = false;
                }
                else
                {
                    count++;
                    categoryItem.IsVisible = true;
                }        
            }
            category.InvalidateState();
            return count;
        }
    }
}
