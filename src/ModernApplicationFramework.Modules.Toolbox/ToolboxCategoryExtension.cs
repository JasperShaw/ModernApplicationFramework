using System;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public static class ToolboxCategoryExtension
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

            if (count > 0)
                category.IsExpanded = true;
            category.InvalidateState();
            return count;
        }
    }
}