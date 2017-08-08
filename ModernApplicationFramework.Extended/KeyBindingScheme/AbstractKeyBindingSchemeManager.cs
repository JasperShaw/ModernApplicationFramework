using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{
    public abstract class AbstractKeyBindingSchemeManager : IKeyBindingSchemeManager
    {      
        protected IKeyGestureService GestureService { get; }
        
        public ICollection<SchemeDefinition> SchemeDefinitions { get; protected set; }
        
        public SchemeDefinition CurrentScheme { get; protected set; }

        protected AbstractKeyBindingSchemeManager()
        {
            SchemeDefinitions = new List<SchemeDefinition>();
            GestureService = IoC.Get<IKeyGestureService>();
        }

        public abstract void LoadSchemeDefinitions();
        
        public void SetScheme(SchemeDefinition definition)
        {
            if (definition == null)
                return;
            var scheme = definition.Load(); 
            if (!SchemeDefinitions.Contains(definition))
                SchemeDefinitions.Add(definition);
            GestureService.RemoveAllKeyGestures();
            foreach (var mapping in scheme.KeyGestureScopeMappings)
                mapping.CommandDefinition.Gestures.Add(mapping.GestureScopeMapping);
            CurrentScheme = definition;
        }

        public abstract void SetScheme();
    }

    [Export(typeof(IKeyBindingSchemeManager))]
    public sealed class DefaultKeyBindingSchemeManager : AbstractKeyBindingSchemeManager
    {
        public override void LoadSchemeDefinitions()
        {
            CreateDefaultScheme();
        }

        public override void SetScheme()
        {
            SetScheme(SchemeDefinitions.FirstOrDefault());
        }

        private void CreateDefaultScheme()
        {
            SchemeDefinitions.Add(new DefaultSchemeDefinition());
        }

        private class DefaultSchemeDefinition : SchemeDefinition
        {
            public DefaultSchemeDefinition() : base("Default")
            {
            }

            public override KeyBindingScheme Load()
            {
                var commands = IoC.GetAll<CommandDefinitionBase>().OfType<CommandDefinition>();
                var list = commands.Where(x => x.DefaultKeyGesture != null || x.DefaultGestureScope != null)
                    .Select(command => new CommandGestureScopeMapping(command,
                        new GestureScopeMapping(command.DefaultGestureScope, command.DefaultKeyGesture)));
                return new KeyBindingScheme(Name, list);
            }
        }
    }
}