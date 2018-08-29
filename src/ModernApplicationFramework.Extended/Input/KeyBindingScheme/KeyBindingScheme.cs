using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Xml;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
{
    public class KeyBindingScheme
    {
        public IReadOnlyCollection<CommandGestureScopeMapping> KeyGestureScopeMappings { get; }

        public string Name { get; }

        public KeyBindingScheme(string name, IEnumerable<CommandGestureScopeMapping> list)
        {
            Name = name;
            KeyGestureScopeMappings = new List<CommandGestureScopeMapping>(list);
        }

        public static KeyBindingScheme LoadFromFile(string filePath)
        {
            var file = new XmlObjectParser<KeyBindingSchemeFile>(filePath).Parse();           
            var list = new List<CommandGestureScopeMapping>();
            var scopes = IoC.GetAll<GestureScope>().ToList();       
            var service = IoC.Get<ICommandBarItemService>();
            if (file.Shortcuts == null)
                return new KeyBindingScheme(file.Name, list);
            foreach (var shortcut in file.Shortcuts)
            {
                var cdb = service.GetItemDefinition("t" ,shortcut.Command);
                if (cdb == null || !(cdb is CommandDefinition cd))
                    continue;
                if (!Guid.TryParse(shortcut.Scope, out var scopeId))
                    continue;  
                var scope =  scopes.FirstOrDefault(x => x.Id.Equals(scopeId));
                try
                {
                    list.Add(new CommandGestureScopeMapping(scope, cd,
                        (MultiKeyGesture)new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.InvariantCulture, shortcut.Value)));
                }
                catch (ArgumentException)
                {
                }
            }
            return new KeyBindingScheme(file.Name, list);
        }
    }
}
