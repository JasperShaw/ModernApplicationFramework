using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.MVVM.Demo
{

    /* TODO: Changeable Gestures
     * So far you can not change a Keygesture. Make this possible at some time.
     */
   
    [Export(typeof(IKeyGestureHandler))]
    public class KeyGestureHandler : IKeyGestureHandler
    {

        private readonly CommandDefinition[] _keyboardShortcuts;

        [ImportingConstructor]
        public KeyGestureHandler([ImportMany] CommandDefinition[] keyboardShortcuts)
        {
            _keyboardShortcuts = keyboardShortcuts;
        }

        public void BindKeyGesture(UIElement uiElement)
        {
            foreach (var gc in from definition in _keyboardShortcuts where definition.Command is GestureCommand select definition.Command as GestureCommand)
                uiElement.InputBindings.Add(new InputBinding(gc, GetPrimaryKeyGesture(gc)));
        }

        public KeyGesture GetPrimaryKeyGesture(GestureCommand command) => command.KeyGesture;
    }
}
