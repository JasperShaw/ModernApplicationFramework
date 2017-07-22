using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    //TODO: Add multi category support
    /// <inheritdoc />
    /// <summary>
    /// A service to manage key input bindings
    /// </summary>
    /// <seealso cref="IKeyGestureHandler" />
    [Export(typeof(IKeyGestureHandler))]
    public class KeyGestureHandler : IKeyGestureHandler
    {
        private readonly CommandDefinition[] _keyboardShortcuts;

        [ImportingConstructor]
        public KeyGestureHandler([ImportMany] CommandDefinitionBase[] keyboardShortcuts)
        {
            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();
        }


        private UIElement _currentElement;

        /// <summary>
        /// Restores the bindings.
        /// </summary>
        public void RestoreBindings()
        {
            if (_currentElement == null)
                return;
            _currentElement.InputBindings.Clear();
            BindKeyGesture(_currentElement);
        }

        /// <summary>
        /// Binds the key gesture.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        public void BindKeyGesture(UIElement uiElement)
        {
            _currentElement = uiElement;
            foreach (var gc in from definition in _keyboardShortcuts
                               where definition.Command is MultiKeyGestureCommandWrapper
                               select definition.Command as MultiKeyGestureCommandWrapper)
                if (gc.KeyGesture != null)
                    uiElement.InputBindings.Add(new InputBinding(gc, GetPrimaryKeyGesture(gc)));
        }

        /// <summary>
        /// Gets the primary key gesture.
        /// </summary>
        /// <param name="abstractCommand">The abstractCommand.</param>
        /// <returns></returns>
        public KeyGesture GetPrimaryKeyGesture(MultiKeyGestureCommandWrapper abstractCommand) => abstractCommand.KeyGesture;
    }
}