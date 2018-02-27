using System.ComponentModel;
using System.Windows.Input;

namespace ModernApplicationFramework.EditorBase.Interfaces.Layout
{
    public interface IDocument : INotifyPropertyChanged
    {
        ICommand SaveFileAsCommand { get; }

        ICommand SaveFileCommand { get; }
    }
}