using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    //Only used for first time build and resetting
    [Export(typeof(ToolboxItemsBuilder))]
    internal class ToolboxItemsBuilder
    {
        private readonly IEnumerable<IToolboxCategory> _categories;
        private readonly IEnumerable<IToolboxItem> _items;

        [ImportingConstructor]
        public ToolboxItemsBuilder([ImportMany] IEnumerable<IToolboxCategory> categories, [ImportMany] IEnumerable<IToolboxItem> items)
        {
            _categories = categories;
            _items = items;
        }

        public IReadOnlyCollection<IToolboxCategory> Build(Type targetType)
        {
            var items = _items.ToList();
            var categories = _categories.Where(x => x.TargetType == targetType).ToList();
            foreach (var category in categories)
            {
                category.Items.Clear();
                var matching = items.Where(x => x.OriginalParent == category);
                category.Items.AddRange(matching);
            }
            categories.Add(ToolboxItemCategory.DefaultCategory);
            return categories;
        }
    }
}
