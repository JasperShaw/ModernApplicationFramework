using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Dialoges;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(INewElementDialogModel))]
    public class NewElementDialogViewModel : Screen, INewElementDialogModel
    {
        private string _name;

        private string _path;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name)
                    return;
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (value == _path)
                    return;
                _path = value;
                NotifyOfPropertyChange();
            }
        }

        public IElementDialogItemPresenter ItemPresenter { get; set; }

        public ICommand ApplyCommand => new Command(Apply, CanApply);

        public ICommand BrowseCommand => new Command(Browse, CanBrowse);
        public object ResultData { get; protected set; }

        private void Apply()
        {
            ResultData = ItemPresenter.CreateResult(Name, Path);
            TryClose(true);
        }

        private void Browse()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            Path = dialog.SelectedPath;
        }


        //TODO
        private bool CanApply()
        {
            return true;
        }

        private bool CanBrowse() => ItemPresenter.UsesPathProperty;
    }
}