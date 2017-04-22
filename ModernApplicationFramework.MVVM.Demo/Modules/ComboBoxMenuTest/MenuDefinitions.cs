using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.MenuDefinitions;
using ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest
{
    public class MenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ComboBoxTestMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 14, "&ComboBox");

        [Export] public static CommandBarGroupDefinition ComboGroup = new CommandBarGroupDefinition(ComboBoxTestMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition ComboItem = new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(ComboGroup, uint.MinValue, false, false, false);


        [Export] public static CommandBarItemDefinition ComboItemMain =
            new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(MainMenuBarDefinition.MainMenuBarGroup,
                uint.MinValue, true, false, true);

        [Export]
        public static CommandBarItemDefinition ComboItemMain2 =
            new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(MainMenuBarDefinition.MainMenuBarGroup,
                uint.MaxValue, false, true, false);
    }
}
