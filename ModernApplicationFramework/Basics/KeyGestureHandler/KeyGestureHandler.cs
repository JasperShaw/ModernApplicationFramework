using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.KeyGestureHandler
{
    [Export(typeof(IKeyGestureHandler))]
    public class KeyGestureHandler : IKeyGestureHandler
    {
        private readonly CommandDefinition[] _keyboardShortcuts;

        [ImportingConstructor]
        public KeyGestureHandler([ImportMany] DefinitionBase[] keyboardShortcuts)
        {
            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();
        }


        private UIElement _currentElement;

        public void RestoreBindings()
        {
            if (_currentElement == null)
                return;
            _currentElement.InputBindings.Clear();
            BindKeyGesture(_currentElement);
        }

        public void BindKeyGesture(UIElement uiElement)
        {
            _currentElement = uiElement;
            foreach (var gc in from definition in _keyboardShortcuts
                               where definition.Command is GestureCommandWrapper
                               select definition.Command as GestureCommandWrapper)
                uiElement.InputBindings.Add(new InputBinding(gc, GetPrimaryKeyGesture(gc)));
        }

        public KeyGesture GetPrimaryKeyGesture(GestureCommandWrapper command) => command.KeyGesture;
    }
}