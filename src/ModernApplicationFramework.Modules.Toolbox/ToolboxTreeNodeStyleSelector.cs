using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxTreeNodeStyleSelector : StyleSelector
    {
        public Style ItemStyle { get; set; }

        public Style CategoryStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IToolboxCategory)
                return CategoryStyle;
            if (item is IToolboxItem)
                return ItemStyle;
            return base.SelectStyle(item, container);
        }
    }
}
