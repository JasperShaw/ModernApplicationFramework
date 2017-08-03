using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Services;
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

    public class CommandGestureScopeMappingViewSource : PropertyBoundFilteringCollectionViewSource<CommandGestureScopeMapping>
    {
        protected override bool AcceptItem(CommandGestureScopeMapping item)
        {
            return true;
        }
    }
}