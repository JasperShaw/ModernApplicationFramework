using System.Windows;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces
{
    public interface ICanHaveInputBindings
    {
        GestureScope GestureScope { get; }
        
        UIElement BindableElement { get; }
    }
}