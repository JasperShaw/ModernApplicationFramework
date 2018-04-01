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
            return base.SelectStyle(item, container);
        }
    }
}
