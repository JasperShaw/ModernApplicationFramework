using System;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItem : ToolboxNodeItem
    {
        public Type TargetType { get; }

        internal ToolboxItemCategory Parent { get; set; }

        public BitmapSource IconSource { get; set; }

        public ToolboxItem(Type targetType, ToolboxItemCategory parent, string name, BitmapSource iconSource = null) : base(name)
        {
            TargetType = targetType;
            IconSource = iconSource;
            parent.Items?.Add(this);
        }
    }
}