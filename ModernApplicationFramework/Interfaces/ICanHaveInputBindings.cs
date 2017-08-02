using System.Windows;

namespace ModernApplicationFramework.Input.Command
{
    public interface ICanHaveInputBindings
    {
        CommandGestureCategory GestureCategory { get; }
        
        UIElement BindableElement { get; }
    }
}