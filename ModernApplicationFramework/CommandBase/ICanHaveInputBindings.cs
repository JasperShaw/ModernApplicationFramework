using System.Windows;

namespace ModernApplicationFramework.CommandBase
{
    public interface ICanHaveInputBindings
    {
        CommandGestureCategory GestureCategory { get; }
        
        UIElement BindableElement { get; }

        void BindGestures();
    }
}