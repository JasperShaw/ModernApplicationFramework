using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.InfoBar.Internal;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal class InfoBarActionItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ButtonTemplate { get; set; }

        public DataTemplate HyperlinkTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is InfoBarActionViewModel barActionViewModel))
                return base.SelectTemplate(item, container);
            return barActionViewModel.IsButton ? ButtonTemplate : HyperlinkTemplate;
        }
    }
}
