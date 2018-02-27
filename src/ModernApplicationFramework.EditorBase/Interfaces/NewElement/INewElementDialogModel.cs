using System.Windows.Input;
using Caliburn.Micro;

namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface INewElementDialogModel : IScreen
    {
        ICommand ApplyCommand { get; }

        ICommand OpenWithCommand { get; }

        ICommand BrowseCommand { get; }

        string Name { get; set; }

        string Path { get; set; }

        string OkButtonText { get; set; }
    }


    public interface INewElementDialogModel<T> : INewElementDialogModel
    {
        T ItemPresenter { get; set; }

        T ResultData { get; }
    }
}