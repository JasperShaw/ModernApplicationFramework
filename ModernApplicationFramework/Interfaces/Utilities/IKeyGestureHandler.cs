using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IKeyGestureHandler
    {
        void BindKeyGesture(UIElement uiElement);

        KeyGesture GetPrimaryKeyGesture(GestureCommandWrapper command);
    }
}