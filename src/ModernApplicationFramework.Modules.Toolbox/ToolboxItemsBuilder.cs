using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox
{
    //Only used for first time build and resetting
    [Export(typeof(ToolboxItemsBuilder))]
    internal class ToolboxItemsBuilder
    {
        private readonly ToolboxItemHost _host;

        [ImportingConstructor]
        public ToolboxItemsBuilder(ToolboxItemHost host)
        {
            _host = host;
        }

        public IReadOnlyCollection<IToolboxCategory> Build(Type targetType)
        {
            var items = _host.AllItems;
            var categories = _host.AllCategories.Where(x => x.TargetType == targetType).ToList();
            foreach (var category in categories)
            {
                category.Items.Clear();
                var matching = items.Where(x => x.OriginalParent == category).ToList();
                //AddRange does currently not work because the triggered event has wrong args
                matching.ForEach(x => category.Items.Add(x));
            }
            categories.AddRange(_host.AllCategories.Where(x => x.IsCustom));
            categories.Add(ToolboxItemCategory.DefaultCategory);
            return categories;
        }
    }
}
