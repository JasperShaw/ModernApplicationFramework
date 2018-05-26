using System;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItem : ToolboxNodeItem
    {
        public Type TargetType { get; }

        internal ToolboxItemCategory Parent { get; set; }

        internal ToolboxItemCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public ToolboxItem(Type targetType, ToolboxItemCategory originalParent, string name, BitmapSource iconSource = null) : base(name)
        {
            TargetType = targetType;
            IconSource = iconSource;
            OriginalParent = originalParent;
        }
    }
}