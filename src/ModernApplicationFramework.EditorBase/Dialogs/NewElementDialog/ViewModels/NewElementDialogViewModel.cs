using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Dialogs.Native;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog.ViewModels
{
    [Export(typeof(INewElementDialogModel))]
    public class NewElementDialogViewModel<T> : Conductor<IExtensionDialogItemPresenter<T>>, INewElementDialogModel
    {
        private string _name;
        private IExtensionDialogItemPresenter<T> _itemPresenter;
        private string _okButtonText;
        private string _path;

        private bool _applied;

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
                    Name = firstOrDefault.TemplateName;
                _itemPresenter.PropertyChanged += _itemPresenter_PropertyChanged;
                _itemPresenter.ItemDoubledClicked += _itemPresenter_ItemDoubledClicked;

                OkButtonText = _itemPresenter.CanOpenWith
                    ? NewElementDialogResources.NewElementOpenButton
                    : NewElementDialogResources.NewElementOkButton;

                ActivateItem(value);
            }
        }

        private void _itemPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemPresenter.SelectedExtension))
                Name = ItemPresenter.SelectedExtension?.TemplateName;

        }

        private void _itemPresenter_ItemDoubledClicked(object sender, ModernApplicationFramework.Core.Events.ItemDoubleClickedEventArgs e)
        {
            if (!CanApply())
                return;
            ApplyCommand.Execute(null);
        }

        public ICommand ApplyCommand => new UICommand(Apply, CanApply);
        public ICommand OpenWithCommand => new UICommand(OpenWith, CanOpenWith);

        public ICommand BrowseCommand => new UICommand(Browse, CanBrowse);

        public T ResultData { get; protected set; }

        private void Apply()
        {
            if (_applied)
                return;
            ResultData = ItemPresenter.CreateResult(Name, Path);
            if (ResultData == null)
                TryClose(false);
            TryClose(true);
            _applied = true;
        }

        private void OpenWith()
        {
            if (!ItemPresenter.CanOpenWith)
                throw new InvalidOperationException("Can not perform OpenWith() action in non OpenWith context");
            ResultData = ItemPresenter.CreateResultOpenWith(Name, Path);
            if (ResultData != null)
                TryClose(true);
        }

        private void Browse()
        {
            var dialog = new NativeFolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            Path = dialog.SelectedPath;
        }

        private bool CanApply()
        {
            if (!ItemPresenter.UsesNameProperty && !ItemPresenter.UsesPathProperty && ItemPresenter.SelectedExtension != null)
                return true;
            var result = !(ItemPresenter.UsesPathProperty && !WindowsFileNameHelper.IsValidPath(Path));
            if (ItemPresenter.UsesNameProperty && !WindowsFileNameHelper.IsValidFileName(Name))
                result = false;
            if (ItemPresenter.SelectedExtension == null)
                result = false;
            return result;
        }

        private bool CanOpenWith()
        {
            return _itemPresenter.CanOpenWith && CanApply();
        }

        private bool CanBrowse() => ItemPresenter.UsesPathProperty;
    }
}