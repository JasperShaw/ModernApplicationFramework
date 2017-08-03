using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public class CommandCategoryGestureMapping
    {
        public CommandDefinition Command { get; }

        public CategoryGestureMapping CategoryGestureMapping { get; }
        
        public string Name => $"{Command.TrimmedCategoryCommandName} ({CategoryGestureMapping.KeyGesture.DisplayString} ({CategoryGestureMapping.Category.Name}))";

        public CommandCategoryGestureMapping(CommandGestureCategory category, CommandDefinition command,
            MultiKeyGesture gesture) : this(command, new CategoryGestureMapping(category, gesture))
        {
        }

        public CommandCategoryGestureMapping(CommandDefinition command, CategoryGestureMapping mapping)
        {
            Command = command;
            CategoryGestureMapping = mapping;
        }
    }
}