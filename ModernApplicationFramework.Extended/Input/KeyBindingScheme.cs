using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;

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

        public static KeyBindingScheme LoadSchemeFromFile(string file)
        {
            return null;
        }
    }

    public interface IKeyBindingSchemeManager
    {
        ICollection<KeyBindingScheme> Schemes { get; }

        void LoadSchemes();
    }

    public abstract class AbstractKeyBindingSchemeManager : IKeyBindingSchemeManager
    {
        protected abstract string SchemeFilesLocation { get; }
        
        protected abstract bool UseLocalFiles { get; }
        
        public ICollection<KeyBindingScheme> Schemes { get; protected set; }

        protected AbstractKeyBindingSchemeManager()
        {
            Schemes = new List<KeyBindingScheme>();
        }

        public abstract void LoadSchemes();
    }

    [Export(typeof(IKeyBindingSchemeManager))]
    public sealed class DefaultKeyBindingSchemeManager : AbstractKeyBindingSchemeManager
    {
        protected override string SchemeFilesLocation => null;
        protected override bool UseLocalFiles => false;

        public DefaultKeyBindingSchemeManager()
        {
            LoadSchemes();
        }

        public override void LoadSchemes()
        {
            CreateDefaultScheme();
        }

        private void CreateDefaultScheme()
        {
            var commands = IoC.GetAll<CommandDefinitionBase>().OfType<CommandDefinition>();
            var list = commands.Where(x => x.DefaultKeyGesture != null || x.DefaultGestureScope != null)
                .Select(command => new CommandGestureScopeMapping(command,
                    new GestureScopeMapping(command.DefaultGestureScope, command.DefaultKeyGesture)));
            Schemes.Add(new KeyBindingScheme("Default", list, true));
        }
    }
}
