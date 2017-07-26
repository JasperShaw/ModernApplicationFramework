using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// A service to manage key input bindings
    /// </summary>
    /// <seealso cref="IKeyGestureService" />
    [Export(typeof(IKeyGestureService))]
    public class KeyGestureService : IKeyGestureService
    {
        private readonly CommandDefinition[] _keyboardShortcuts;

        //protected CommandDefinitionGestureMappingStore BindingStore { get; } = new CommandDefinitionGestureMappingStore();

        [ImportingConstructor]
        public KeyGestureService([ImportMany] CommandDefinitionBase[] keyboardShortcuts)
        {
            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();
            
            LoadDefaults();
        }

        public void BindKeyGestures(ICanHaveInputBindings hostingModel)
        {
            var categoryCommands = _keyboardShortcuts.Where(x => 
                x.DefaultGestureCategory != null &&
                x.DefaultGestureCategory.Equals(hostingModel.GestureCategory));


            foreach (var gc in categoryCommands)
                if (gc.DefaultGestureCategory != null)
                {
                    hostingModel.BindableElement.InputBindings.Add(new InputBinding(gc.Command, gc.DefaultKeyGesture));
                }
        }

        /// <inheritdoc />
        /// <summary>
        /// Loads all available key gestures and applies them to their command
        /// </summary>
        public virtual void Load()
        {
            
        }

        public virtual void LoadDefaults()
        {
            var defaultCommands =
                _keyboardShortcuts.Where(x => x.DefaultGestureCategory != null && x.DefaultKeyGesture != null);

            foreach (var cd in defaultCommands)
            {
                cd.Gestures.Clear();
                cd.Gestures.Add(cd.DefaultGestureCategory, cd.DefaultKeyGesture);
            }
        }

        public IEnumerable<CommandCategoryGestureMapping> GetAllBindings()
        {
            return from commandDefinition in _keyboardShortcuts
                from commandDefinitionGesture in commandDefinition.Gestures
                select new CommandCategoryGestureMapping(commandDefinitionGesture.Key, commandDefinition,
                    commandDefinitionGesture.Value);
        }
    }

    public class CommandCategoryGestureMapping
    {
        public CommandCategoryGestureMapping(CommandGestureCategory category, CommandDefinitionBase command, KeyGesture gesture)
        {
            Category = category;
            Command = command;
            Gesture = gesture;
        }

        public CommandDefinitionBase Command { get; }
        public KeyGesture Gesture { get; }
        public CommandGestureCategory Category { get; }

        public override string ToString()
        {
            string gestureText;

            if (Gesture is MultiKeyGesture)
                gestureText = (string) MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture,
                    Gesture,
                    typeof(string));
            else
                gestureText = Gesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
            return $"{Command.Name} + {Category.Name} = {gestureText}";
        }
    }
}