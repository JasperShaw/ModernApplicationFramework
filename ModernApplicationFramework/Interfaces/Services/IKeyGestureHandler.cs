using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A <see cref="IKeyGestureHandler"/> administrates Keybindings to a <see cref="UIElement"/>
    /// </summary>
    public interface IKeyGestureHandler
    {
        void BindKeyGesture(UIElement uiElement);

        void RestoreBindings();

        KeyGesture GetPrimaryKeyGesture(UICommand abstractCommand);

        void BindKeyGestures(ICanHaveInputBindings hostingModel);
    }
}