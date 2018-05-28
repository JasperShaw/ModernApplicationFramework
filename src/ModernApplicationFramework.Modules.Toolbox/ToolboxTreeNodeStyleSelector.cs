using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxTreeNodeStyleSelector : StyleSelector
    {
        public Style ItemStyle { get; set; }

        public Style CategoryStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolboxItemCategory)
                return CategoryStyle;
            if (item is IToolboxItem)
                return ItemStyle;
            return base.SelectStyle(item, container);
        }
    }
}
