using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.NewElementDialog.ViewModels
{
    [Export(typeof(INewElementDialogModel))]
    public class NewElementDialogViewModel<T> : Conductor<IExtensionDialogItemPresenter<T>>, INewElementDialogModel
    {
        private string _name;
        private IExtensionDialogItemPresenter<T> _itemPresenter;
        private string _okButtonText;
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

        public string OkButtonText
        {
            get => _okButtonText;
            set
            {
                if (value == _okButtonText) return;
                _okButtonText = value;
                NotifyOfPropertyChange();
            }
        }


        public IExtensionDialogItemPresenter<T> ItemPresenter
        {
            get => _itemPresenter;
            set
            {
                if (value == _itemPresenter)
                    return;
                if (_itemPresenter != null)
                {
                    _itemPresenter.PropertyChanged -= _itemPresenter_PropertyChanged;
                    _itemPresenter.ItemDoubledClicked -= _itemPresenter_ItemDoubledClicked;
                }
                _itemPresenter = value;
                var firstOrDefault = _itemPresenter.Extensions?.FirstOrDefault();
                if (firstOrDefault != null)
                    Name = firstOrDefault.PresetElementName;
                _itemPresenter.PropertyChanged += _itemPresenter_PropertyChanged;
                _itemPresenter.ItemDoubledClicked += _itemPresenter_ItemDoubledClicked;

                OkButtonText = _itemPresenter.CanOpenWith ? "Open" : "OK";

                ActivateItem(value);
            }
        }

        private void _itemPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemPresenter.SelectedExtension))
                Name = ItemPresenter.SelectedExtension.PresetElementName;

        }

        private void _itemPresenter_ItemDoubledClicked(object sender, ModernApplicationFramework.Core.Events.ItemDoubleClickedEventArgs e)
        {
            if (!CanApply())
                return;
            Apply();
        }

        public ICommand ApplyCommand => new UICommand(Apply, CanApply);

        public ICommand BrowseCommand => new UICommand(Browse, CanBrowse);

        public T ResultData { get; protected set; }

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
            if (!ItemPresenter.UsesNameProperty && !ItemPresenter.UsesPathProperty && ItemPresenter.SelectedExtension != null)
                return true;
            bool result = !(ItemPresenter.UsesPathProperty && !WindowsFileNameHelper.IsValidPath(Path));
            if (ItemPresenter.UsesNameProperty && !WindowsFileNameHelper.IsValidFileName(Name))
                result = false;
            if (ItemPresenter.SelectedExtension == null)
                result = false;
            return result;
        }

        private bool CanBrowse() => ItemPresenter.UsesPathProperty;
    }
}