using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Dialoges;
using ModernApplicationFramework.MVVM.Interfaces;
using static ModernApplicationFramework.Core.Utilities.WindowsFileNameHelper;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(INewElementDialogModel))]
    public class NewElementDialogViewModel : Screen, INewElementDialogModel
    {
        private string _name;

        private string _path;

        public string Name
        {
            get => _name;
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
            get => _path;
            set
            {
                if (value == _path)
                    return;
                _path = value;
                NotifyOfPropertyChange();
            }
        }

        private IExtensionDialogItemPresenter _itemPresenter;

        public IExtensionDialogItemPresenter ItemPresenter
        {
            get => _itemPresenter;
            set
            {
                if (value == _itemPresenter)
                    return;
                if (_itemPresenter != null)
                {
                    _itemPresenter.OnSelectedItemChanged -= _itemPresenter_OnSelectedItemChanged;
                    _itemPresenter.ItemDoubledClicked -= _itemPresenter_ItemDoubledClicked;
                }
                _itemPresenter = value;
                var firstOrDefault = _itemPresenter.ItemSource.FirstOrDefault();
                if (firstOrDefault != null)
                    Name = firstOrDefault.PresetElementName;
                _itemPresenter.OnSelectedItemChanged += _itemPresenter_OnSelectedItemChanged;
                _itemPresenter.ItemDoubledClicked += _itemPresenter_ItemDoubledClicked;
            }
        }

        private void _itemPresenter_ItemDoubledClicked(object sender, ModernApplicationFramework.Core.Events.ItemDoubleClickedEventArgs e)
        {
            if (!CanApply())
                return;
            Apply();
        }

        private void _itemPresenter_OnSelectedItemChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            Name = ItemPresenter.SelectedItem.PresetElementName;
        }

        public ICommand ApplyCommand => new CommandWrapper(Apply, CanApply);

        public ICommand BrowseCommand => new CommandWrapper(Browse, CanBrowse);
        public object ResultData { get; protected set; }

        private void Apply()
        {
            ResultData = ItemPresenter.CreateResult(Name, Path);
            if (ResultData == null)
                TryClose(false);
            TryClose(true);
        }

        private void Browse()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            Path = dialog.SelectedPath;
        }

        private bool CanApply()
        {
            var result = !(ItemPresenter.UsesPathProperty && !IsValidPath(Path));
            if (ItemPresenter.UsesNameProperty && !IsValidFileName(Name))
                result = false;
            if (ItemPresenter.SelectedItem == null)
                result = false;
            return result;
        }

        private bool CanBrowse() => ItemPresenter.UsesPathProperty;
    }
}