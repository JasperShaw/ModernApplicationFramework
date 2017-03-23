using System.Windows.Input;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IDocument : ILayoutItem
    {
        ICommand SaveFileAsCommand { get; }

        ICommand SaveFileCommand { get; }
    }
}