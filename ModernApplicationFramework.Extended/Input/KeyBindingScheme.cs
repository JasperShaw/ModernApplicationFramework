using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Extended.Input
{
    public class KeyBindingScheme
    {
        public string Name { get; }
       
        public IEnumerable<CommandGestureScopeMapping> KeyGestureScopeMappings { get; }

        public KeyBindingScheme(string name, KeyBindingScheme baseScheme)
        {
            
        }
        
        public KeyBindingScheme(string name, IEnumerable<CommandGestureScopeMapping> list)
        {
            
        }

        public static KeyBindingScheme LoadSchemeFromFile(string file)
        {
            return null;
        }
    }
}
