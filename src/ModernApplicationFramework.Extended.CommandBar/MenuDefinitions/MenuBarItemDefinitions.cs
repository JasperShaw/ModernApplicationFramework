using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public class MenuBarItemDefinitions
    {
        [Export] public static CommandBarItem FullScreenTopMenuItem =
            new CommandBarCommandItem<FullScreenCommandDefinition>(new Guid("{53AC01F9-CBA7-49D8-A746-D6FE0B37CF35}"), MainMenuBarDefinition.MainMenuBarGroup,
                uint.MaxValue, CommandBarFlags.CommandFlagPictAndText | CommandBarFlags.CommandNoCustomize, false, true);

        static MenuBarItemDefinitions()
        {
            var myBinding = new Binding(nameof(CommandBarDataSource.IsVisible))
            {
                Source = FullScreenTopMenuItem.ItemDataSource,
                Mode = BindingMode.OneWayToSource
            };
            ((ModernChromeWindow)Application.Current.MainWindow).SetBinding(ModernChromeWindow.FullScreenProperty,
                myBinding);
        }
    }
}