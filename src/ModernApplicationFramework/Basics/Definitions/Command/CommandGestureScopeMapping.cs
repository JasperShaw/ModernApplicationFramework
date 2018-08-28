using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using System.Collections.Generic;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public class CommandGestureScopeMapping
    {
        public CommandDefinition CommandDefinition { get; }

        public GestureScopeMapping GestureScopeMapping { get; }
        
        public string Name => $"{CommandDefinition.TrimmedCategoryCommandName} ({GestureScopeMapping.KeyGesture.DisplayString} ({GestureScopeMapping.Scope.Text}))";

        public CommandGestureScopeMapping(GestureScope scope, CommandDefinition command,
            MultiKeyGesture gesture) : this(command, new GestureScopeMapping(scope, gesture))
        {
        }

        public CommandGestureScopeMapping(CommandDefinition commandDefinition, GestureScopeMapping scopeMapping)
        {
            CommandDefinition = commandDefinition;
            GestureScopeMapping = scopeMapping;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is CommandGestureScopeMapping cgsm)
                return Equals(cgsm);
            return false;
        }

        protected bool Equals(CommandGestureScopeMapping mapping)
        {
            return mapping.CommandDefinition == CommandDefinition &&
                   mapping.GestureScopeMapping.Equals(GestureScopeMapping);
        }

        public override int GetHashCode()
        {
            var hashCode = 1580834471;
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandDefinition>.Default.GetHashCode(CommandDefinition);
            hashCode = hashCode * -1521134295 + EqualityComparer<GestureScopeMapping>.Default.GetHashCode(GestureScopeMapping);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}