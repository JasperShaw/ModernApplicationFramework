using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class StyleUtilities
    {
        internal static void SelectStyleForItem(FrameworkElement element, object item, IExposeStyleKeys styleKeySource)
        {
            var control = element as Control;
            var context = control?.DataContext as CommandBarDefinitionBase;
            if (context == null)
                return;
            ResourceKey key = null;

            if (context.CommandDefinition == null)
            {
                key = styleKeySource.MenuStyleKey;
                element.SetResourceReference(FrameworkElement.StyleProperty, key);
                return;
            }

            switch (context.CommandDefinition.ControlType)
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
                case CommandControlTypes.MenuToolbar:
                    key = styleKeySource.MenuControllerStyleKey;
                    break;
            }
            if (key == null)
                return;
            element.SetResourceReference(FrameworkElement.StyleProperty, key);
        }
    }
}
