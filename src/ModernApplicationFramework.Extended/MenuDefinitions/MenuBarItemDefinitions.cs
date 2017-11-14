using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public class MenuBarItemDefinitions
    {
        [Export] public static CommandBarItemDefinition FullScreenTopMenuItem =
            new CommandBarCommandItemDefinition<FullScreenCommandDefinition>(new Guid("{53AC01F9-CBA7-49D8-A746-D6FE0B37CF35}"), MainMenuBarDefinition.MainMenuBarGroup,
                uint.MaxValue, false, true, false, false, false);

        static MenuBarItemDefinitions()
        {
            var myBinding = new Binding(nameof(CommandBarItemDefinition.IsVisible))
            {
                Source = IoC.GetAll<CommandBarItemDefinition>().FirstOrDefault(x => x.CommandDefinition.Id == new Guid("{9EE995EC-45C6-40B9-A3D6-8A9F486D59C9}")),
                Mode = BindingMode.OneWayToSource
            };
            ((ModernChromeWindow) Application.Current.MainWindow).SetBinding(ModernChromeWindow.FullScreenProperty,
                myBinding);
        }
    }
}