using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public class CommandGestureScopeMapping
    {
        public CommandDefinition CommandDefinition { get; }

        public GestureScopeMapping GestureScopeMapping { get; }
        
        public string Name => $"{CommandDefinition.TrimmedCategoryCommandName} ({GestureScopeMapping.KeyGesture.DisplayString} ({GestureScopeMapping.Scope.Name}))";

        public CommandGestureScopeMapping(GestureScope scope, CommandDefinition command,
            MultiKeyGesture gesture) : this(command, new GestureScopeMapping(scope, gesture))
        {
        }

        public CommandGestureScopeMapping(CommandDefinition commandDefinition, GestureScopeMapping scopeMapping)
        {
            CommandDefinition = commandDefinition;
            GestureScopeMapping = scopeMapping;
        }
    }
}