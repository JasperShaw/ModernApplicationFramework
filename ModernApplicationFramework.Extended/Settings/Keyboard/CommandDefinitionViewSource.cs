using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Controls.Utilities;

namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    public class CommandDefinitionViewSource : PropertyBoundFilteringCollectionViewSource<CommandDefinition>
    {
        protected override bool AcceptItem(CommandDefinition item)
        {
            return true;
        }
    }
}