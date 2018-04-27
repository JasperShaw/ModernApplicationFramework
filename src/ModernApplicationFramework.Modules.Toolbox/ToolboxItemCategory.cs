using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Modules.Toolbox
{

    public abstract class ToolboxNodeItem
    {
        public string Name { get; }

        protected ToolboxNodeItem(string name)
        {
            Name = name;
        }
    }


    public class ToolboxItemCategory : ToolboxNodeItem
    {
        public Type TargetType { get; }

        public ObservableCollection<IToolboxItem> Items { get; }

        public ToolboxItemCategory(Type targetType, string name) : base(name)
        {
            TargetType = targetType;
            Items = new ObservableCollection<IToolboxItem>();
        }
    }

    public interface IToolboxItem
    {
        ToolboxItemCategory Parent { get; }

        string Name { get; }

        Uri IconSource { get; set; }
    }

    public class ToolboxItemEx : ToolboxNodeItem, IToolboxItem
    {
        public Type TargetType { get; }
        public ToolboxItemCategory Parent { get; }
        public Uri IconSource { get; set; }

        public ToolboxItemEx(Type targetType, ToolboxItemCategory parent, string name, Uri iconSource = null) : base(name)
        {
            TargetType = targetType;
            Parent = parent;
            IconSource = iconSource;
            parent.Items?.Add(this);
        }
    }
}
