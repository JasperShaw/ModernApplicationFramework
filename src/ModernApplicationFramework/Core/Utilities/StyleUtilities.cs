using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class StyleUtilities
    {
        /// <summary>
        /// Selects the style for item based on the <see cref="IExposeStyleKeys"/> resource key
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <param name="styleKeySource">The style key.</param>
        public static void SelectStyleForItem(FrameworkElement element, object item, IExposeStyleKeys styleKeySource)
        {
            var control = element as Control;
            var context = control?.DataContext as CommandBarDataSource;
            if (context == null)
                return;
            ResourceKey key = null;

            //if (context.CommandDefinition == null)
            //{
            //    key = styleKeySource.MenuStyleKey;
            //    element.SetResourceReference(FrameworkElement.StyleProperty, key);
            //    return;
            //}

            switch (context.UiType)
            {
                case CommandControlTypes.Separator:
                    key = styleKeySource.SeparatorStyleKey;
                    break;
                case CommandControlTypes.Button:
                case CommandControlTypes.SplitDropDown:
                    key = styleKeySource.ButtonStyleKey;
                    break;
                case CommandControlTypes.Combobox:
                    key = styleKeySource.ComboBoxStyleKey;
                    break;
                case CommandControlTypes.Menu:
                    key = styleKeySource.MenuStyleKey;
                    break;
                case CommandControlTypes.MenuController:
                    key = styleKeySource.MenuControllerStyleKey;
                    break;
            }
            if (key == null)
                return;
            element.SetResourceReference(FrameworkElement.StyleProperty, key);
        }
    }
}
