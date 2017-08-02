using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Input
{
    internal struct CommandCategoryKey : IEquatable<CommandCategoryKey>
    {
        public CommandDefinition Command { get; }
        public CommandGestureCategory Category { get; }

        public CommandCategoryKey(CommandDefinition command, CommandGestureCategory category)
        {
            Command = command;
            Category = category;
        }

        public bool Equals(CommandCategoryKey other)
        {
            return Equals(Command, other.Command) && Equals(Category, other.Category);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is CommandCategoryKey && Equals((CommandCategoryKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Command != null ? Command.GetHashCode() : 0) * 397) ^ (Category != null ? Category.GetHashCode() : 0);
            }
        }
    }
}
