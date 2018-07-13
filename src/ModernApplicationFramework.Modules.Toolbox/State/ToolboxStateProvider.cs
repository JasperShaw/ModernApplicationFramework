using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateProvider))]
    internal class ToolboxStateProvider : IToolboxStateProvider
    {
        public IObservableCollection<IToolboxCategory> ItemsSource { get; }

        [ImportingConstructor]
        public ToolboxStateProvider([ImportMany] IEnumerable<IToolboxCategory> categories, [ImportMany] IEnumerable<IToolboxItem> items)
        {
            var itemSource = new BindableCollection<IToolboxCategory>(categories);

            foreach (var category in itemSource)
            {
                var categoryItems = items.Where(x => x.OriginalParent == category);
                categoryItems.ForEach(x => category.Items.Add(x));
            }
            ItemsSource = itemSource;
        }
    }
}
