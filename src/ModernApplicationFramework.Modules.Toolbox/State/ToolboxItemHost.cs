using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(ToolboxItemHost))]
    public class ToolboxItemHost
    {

        public IReadOnlyCollection<IToolboxCategory> AllCategories => new List<IToolboxCategory>(_categories.ToList().Concat(_customCategories));
        public IReadOnlyCollection<IToolboxItem> AllItems => new List<IToolboxItem>(_items.ToList().Concat(_customItems));

        private ObservableCollection<IToolboxCategory> _categories { get; }
        private ObservableCollection<IToolboxCategory> _customCategories { get; }
        private ObservableCollection<IToolboxItem> _items { get; }
        private ObservableCollection<IToolboxItem> _customItems { get; }


        [ImportingConstructor]
        public ToolboxItemHost([ImportMany] IEnumerable<IToolboxCategory> categories, [ImportMany] IEnumerable<IToolboxItem> items)
        {
            _categories = new ObservableCollection<IToolboxCategory>(categories);
            _items = new ObservableCollection<IToolboxItem>(items);
            _customCategories = new ObservableCollection<IToolboxCategory>();
            _customItems = new ObservableCollection<IToolboxItem>();
        }

        public void RegisterNode(IToolboxNode node)
        {
            if (!node.IsCustom)
                return;

            if(node is IToolboxCategory category)
                if (!_customCategories.Contains(category))
                    _customCategories.Add(category);
        }

        public void DeleteNode(IToolboxNode node)
        {
            if (node is IToolboxCategory category)
                if (_customCategories.Contains(category))
                    _customCategories.Remove(category);
        }
    }
}
