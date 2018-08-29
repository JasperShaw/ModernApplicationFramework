using System;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Input
{
    internal struct CommandCategoryKey : IEquatable<CommandCategoryKey>
    {
        public CommandDefinition Command { get; }
        public GestureScope Scope { get; }

        public CommandCategoryKey(CommandDefinition command, GestureScope scope)
        {
            Command = command;
            Scope = scope;
        }

        public bool Equals(CommandCategoryKey other)
        {
            return Equals(Command, other.Command) && Equals(Scope, other.Scope);
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
                return ((Command != null ? Command.GetHashCode() : 0) * 397) ^ (Scope != null ? Scope.GetHashCode() : 0);
            }
        }
    }
}
