using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Core.Utilities;
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

        public void Initialize()
        {
            foreach (var category in _host.AllCategories)
            {
                var items = _host.AllItems.Where(x => x.OriginalParent == category);
                items.ForEach(x => category.Items.Add(x));
            }
        }
    }
}
