using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest
{
    public static class ToolbarDefinitions
    {
        [Export] public static ToolbarDefinition ComboBox = new ToolbarDefinition("ComboBoxTest", 0, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition Group1 = new CommandBarGroupDefinition(ComboBox, 0);

        [Export] public static CommandBarItemDefinition ComboItem = new CommandBarCommandItemDefinition<ComboBoxCommandDefinition>(Group1, 0);
    }
}
