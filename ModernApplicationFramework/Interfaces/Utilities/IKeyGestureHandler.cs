using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IKeyGestureHandler
    {
        void BindKeyGesture(UIElement uiElement);

        KeyGesture GetPrimaryKeyGesture();
    }
}
