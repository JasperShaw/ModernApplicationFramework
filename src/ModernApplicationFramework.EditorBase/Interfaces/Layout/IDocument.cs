using System.Windows.Input;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces.Layout
{
    public interface IDocument : ILayoutItem
    {
        ICommand SaveFileAsCommand { get; }

        ICommand SaveFileCommand { get; }
    }
}