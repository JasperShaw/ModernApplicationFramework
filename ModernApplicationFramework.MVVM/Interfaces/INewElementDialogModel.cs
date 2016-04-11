using System.Windows.Input;
using ModernApplicationFramework.Caliburn.Interfaces;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface INewElementDialogModel : IScreen
    {
        ICommand ApplyCommand { get; }

        ICommand BrowseCommand { get; }

        object ResultData { get; }

        IExtensionDialogItemPresenter ItemPresenter { get; set; }

        string Name { get; set; }

        string Path { get; set; }
    }
}