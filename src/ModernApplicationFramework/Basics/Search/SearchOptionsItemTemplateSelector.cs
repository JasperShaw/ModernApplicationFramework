using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Basics.Search
{
    public class SearchOptionsItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement frameworElement && item is SearchOptionDataSource searchOption)
            {
                switch (searchOption.Type)
                {
                    case SearchOptionType.Boolean:
                        return frameworElement.FindResource(SearchOptionItemTemplates.Boolean) as DataTemplate;
                    case SearchOptionType.Command:
                        return frameworElement.FindResource(SearchOptionItemTemplates.Command) as DataTemplate;
                }
            }
            return null;
        }
    }
}
