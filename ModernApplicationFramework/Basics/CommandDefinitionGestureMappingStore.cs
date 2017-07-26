using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics
{
    public class CommandDefinitionGestureMappingStore
    {
        private readonly Dictionary<CommandCategoryKey, KeyGesture> _store;
              
        public CommandDefinitionGestureMappingStore()
        {
            _store = new Dictionary<CommandCategoryKey, KeyGesture>();
        }
        
        public void AddMapping(CommandDefinition command, CommandGestureCategory category, KeyGesture keyGesture)
        {
            var s = new CommandCategoryKey(command, category);
            
            if (!_store.ContainsKey(s))
                _store.Add(s, keyGesture);
            else
                _store[s] = keyGesture;
            
        }

        public IEnumerable<CommandCategoryGestureMapping> GetAllMappings()
        {
            return _store.Select(entry => new CommandCategoryGestureMapping(entry.Key.Category, entry.Key.Command, entry.Value));
        }

        public void ClearAllMappings()
        {
            
        }
        
    }

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
