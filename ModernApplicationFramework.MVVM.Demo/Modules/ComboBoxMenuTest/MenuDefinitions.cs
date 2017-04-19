using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.MenuDefinitions;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest
{
    public class MenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ComboBoxTestMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 14, "&ComboBox");

        [Export] public static CommandBarGroupDefinition ComboGroup = new CommandBarGroupDefinition(ComboBoxTestMenu, int.MaxValue);

    }
}
