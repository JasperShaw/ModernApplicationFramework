using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Core
{
    public class TreeNodeStyleSelector : StyleSelector
    {
        public Style ProviderStyle { get; set; }

        public Style CategoryStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is INewElementExtensionsProvider)
                return ProviderStyle;
            if (item is INewElementExtensionTreeNode)
                return CategoryStyle;
            return base.SelectStyle(item, container);
        }
    }
}
