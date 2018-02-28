using System.ComponentModel;
using System.Windows.Input;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IDocument : INotifyPropertyChanged
    {
        ICommand SaveFileAsCommand { get; }

        ICommand SaveFileCommand { get; }
    }
}