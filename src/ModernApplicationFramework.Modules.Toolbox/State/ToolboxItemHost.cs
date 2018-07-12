using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(ToolboxItemHost))]
    internal class ToolboxItemHost
    {

        public IReadOnlyCollection<IToolboxCategory> AllCategories => new List<IToolboxCategory>(Categories.ToList());//.Concat(CustomCategories));
        public IReadOnlyCollection<IToolboxItem> AllItems => new List<IToolboxItem>(Items.ToList());//.Concat(CustomItems));

        private ObservableCollection<IToolboxCategory> Categories { get; }
        //private ObservableCollection<IToolboxCategory> CustomCategories { get; }
        private ObservableCollection<IToolboxItem> Items { get; }
        //private ObservableCollection<IToolboxItem> CustomItems { get; }


        [ImportingConstructor]
        public ToolboxItemHost([ImportMany] IEnumerable<IToolboxCategory> categories, [ImportMany] IEnumerable<IToolboxItem> items)
        {
            Categories = new ObservableCollection<IToolboxCategory>(categories);
            Items = new ObservableCollection<IToolboxItem>(items);
            //CustomCategories = new ObservableCollection<IToolboxCategory>();
            //CustomItems = new ObservableCollection<IToolboxItem>();
        }

        internal void Initialize()
        {
            foreach (var category in AllCategories)
            {
                var items = AllItems.Where(x => x.OriginalParent == category);
                items.ForEach(x => category.Items.Add(x));
            }
        }

        public void RegisterNode(IToolboxNode node)
        {
            if (!node.IsCustom)
                return;

            if(node is IToolboxCategory category)
                if (!Categories.Contains(category))
                    Categories.Add(category);

            if (node is IToolboxItem item)
                if (!Items.Contains(item))
                    Items.Add(item);
        }

        public void DeleteNode(IToolboxNode node)
        {
            if (node is IToolboxCategory category)
                if (Categories.Contains(category))
                    Categories.Remove(category);
            if (node is IToolboxItem item)
                if (Items.Contains(item))
                    Items.Remove(item);
        }
    }
}
