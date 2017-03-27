using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Controls
{
    public class CommandDefinitionButton : System.Windows.Controls.Button
    {
        public CommandDefinitionButton(ToolbarItemDefinition definition)
        {
            DataContext = definition;
        }
    }
}
