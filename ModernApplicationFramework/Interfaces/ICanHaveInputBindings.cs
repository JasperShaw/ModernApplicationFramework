using System.Windows;

namespace ModernApplicationFramework.Input.Command
{
    public interface ICanHaveInputBindings
    {
        GestureScope GestureScope { get; }
        
        UIElement BindableElement { get; }
    }
}