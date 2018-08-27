using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public class MenuBarItemDefinitions
    {
        [Export] public static CommandBarItem FullScreenTopMenuItem =
            new CommandBarCommandItem<FullScreenCommandDefinition>(new Guid("{53AC01F9-CBA7-49D8-A746-D6FE0B37CF35}"), MainMenuBarDefinition.MainMenuBarGroup,
                uint.MaxValue, CommandBarFlags.CommandFlagPictAndText, false, true);

        static MenuBarItemDefinitions()
        {
            //TODO: Full screen binding
            //var myBinding = new Binding(nameof(CommandBarItemDataSource.IsVisible))
            //{
            //    Source = IoC.GetAll<CommandBarItemDataSource>().FirstOrDefault(x => x.CommandDefinition.Id == new Guid("{9EE995EC-45C6-40B9-A3D6-8A9F486D59C9}")),
            //    Mode = BindingMode.OneWayToSource
            //};
            //((ModernChromeWindow) Application.Current.MainWindow).SetBinding(ModernChromeWindow.FullScreenProperty,
            //    myBinding);
        }
    }
}