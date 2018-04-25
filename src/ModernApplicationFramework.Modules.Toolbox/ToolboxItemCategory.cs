using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemCategory
    {
        public Type TargetType { get; }

        public string Name { get; }

        public ObservableCollection<IToolboxItem> Items { get; }

        public ToolboxItemCategory(Type targetType, string name)
        {
            TargetType = targetType;
            Name = name;
            Items = new ObservableCollection<IToolboxItem>();
        }
    }

    public interface IToolboxItem
    {
        ToolboxItemCategory Parent { get; }

        string Name { get; }

        Uri IconSource { get; set; }
    }

    public class ToolboxItemEx : IToolboxItem
    {
        public Type TargetType { get; }
        public ToolboxItemCategory Parent { get; }
        public string Name { get; }
        public Uri IconSource { get; set; }

        public ToolboxItemEx(Type targetType, ToolboxItemCategory parent, string name, Uri iconSource = null)
        {
            TargetType = targetType;
            Parent = parent;
            Name = name;
            IconSource = iconSource;
            parent.Items?.Add(this);
        }
    }
}
