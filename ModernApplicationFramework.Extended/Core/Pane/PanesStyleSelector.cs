using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.Pane
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle { get; set; }

        public Style ToolStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ITool)
                return ToolStyle;

            if (item is ILayoutItem)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}