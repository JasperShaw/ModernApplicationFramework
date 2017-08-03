using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics
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

        public static KeyBindingScheme CreateSchemeFromFile(string file)
        {
            return null;
        }
    }
}
