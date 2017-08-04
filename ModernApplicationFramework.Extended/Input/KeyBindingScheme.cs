using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Extended.Input
{
    public class KeyBindingScheme
    {
        public string Name { get; }
        
        public bool IsDefault { get; }
       
        public IReadOnlyCollection<CommandGestureScopeMapping> KeyGestureScopeMappings { get; }
       
        public KeyBindingScheme(string name, IEnumerable<CommandGestureScopeMapping> list, bool isDefault = false)
        {
            Name = name;
            IsDefault = isDefault;
            KeyGestureScopeMappings = new List<CommandGestureScopeMapping>(list);
        }
    }
}
