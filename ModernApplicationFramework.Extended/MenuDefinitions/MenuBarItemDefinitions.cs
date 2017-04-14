using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public class MenuBarItemDefinitions
    {
        [Export] public static CommandBarGroupDefinition TopLevelMenuItemGroup =
            new CommandBarGroupDefinition(MainMenuBarDefinition.MainMenuBar, uint.MaxValue);

        [Export] public static CommandBarItemDefinition FullScreenTopMenuItem =
            new CommandBarCommandItemDefinition<FullScreenCommandDefinition>(TopLevelMenuItemGroup, uint.MinValue, false, true);

        static MenuBarItemDefinitions()
        {
            var myBinding = new Binding(nameof(CommandBarItemDefinition.IsVisible))
            {
                Source = FullScreenTopMenuItem,
                Mode = BindingMode.OneWayToSource
            };
            ((ModernChromeWindow) Application.Current.MainWindow).SetBinding(ModernChromeWindow.FullScreenProperty,
                myBinding);
            FullScreenTopMenuItem.Flags.TextOnly = true;
            FullScreenTopMenuItem.Flags.Pict = true;
        }
    }
}