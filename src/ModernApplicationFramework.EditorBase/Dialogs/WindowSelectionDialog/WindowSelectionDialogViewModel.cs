using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Core;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Settings.WindowSelectionDialog;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Dialogs.WindowSelectionDialog
{
    [Export(typeof(IWindowSelectionDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class WindowSelectionDialogViewModel : Screen, IWindowSelectionDialogViewModel, IWindowSelectionDialogViewModelInternal
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        private readonly IWindowSelectionDialogSettings _settings;
        private string _messageText;
        private int _dialogWidth;
        private int _dialogHeight;
        private int _nameColumnWidth;
        private IRestorableGridColumnControl _columnControl;

        public ObservableCollection<ILayoutItemBase> OpenLayoutItems { get; }

        public ObservableCollection<ILayoutItemBase> SelectedLayoutItems { get; set; }

        public int DialogWidth
        {
            get => _dialogWidth;
            set
            {
                if (value == _dialogWidth) return;
                _dialogWidth = value;
                NotifyOfPropertyChange();
            }
        }

        public int DialogHeight
        {
            get => _dialogHeight;
            set
            {
                if (value == _dialogHeight) return;
                _dialogHeight = value;
                NotifyOfPropertyChange();
            }
        }

        public int NameColumnWidth
        {
            get => _nameColumnWidth;
            set
            {
                if (value == _nameColumnWidth) return;
                _nameColumnWidth = value;
                NotifyOfPropertyChange();
            }
        }

        public int LastSelectedColumnIndex { get; set; }

        public ListSortDirection LastSortDirection { get; set; }

        public string MessageText
        {
            get => _messageText;
            set
            {
                if (value == _messageText) return;
                _messageText = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand SaveCommand => new UICommand(SaveLayoutItem, CanSaveLayoutItem);

        public ICommand CloseCommand => new UICommand(CloseLayoutItem, CanCloseLayoutItem);

        public ICommand DoubleClickCommand => new DelegateCommand(DoubleClick, CanDoubleClick);

        public ICommand ActivateCommand => new UICommand(ActivateLayoutItem, CanActivateLayoutItem);

        public ICommand OkCommand => new UICommand(OkClick, CanOkClick);

        [ImportingConstructor]
        public WindowSelectionDialogViewModel(IDockingHostViewModel dockingHostViewModel, IWindowSelectionDialogSettings settings)
        {
            DisplayName = WindowSelectionDialogResources.DialogTitle;
            _dockingHostViewModel = dockingHostViewModel;
            _settings = settings;
            OpenLayoutItems =
                new ObservableCollection<ILayoutItemBase>(_dockingHostViewModel.DockingHostView
                    .AllOpenLayoutItemsAsDocuments);


            SelectedLayoutItems = new ObservableCollection<ILayoutItemBase> { OpenLayoutItems.FirstOrDefault(x => x.IsActive) };
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            DialogWidth = _settings.WindowSelectionDialogWidth;
            DialogHeight = _settings.WindowSelectionDialogHeight;
            NameColumnWidth = _settings.NameColumnWidth;

            if (!(view is IRestorableGridColumnControl columnControl))
                return;
            _columnControl = columnControl;
            _columnControl.Reset(_settings.LastSelectedColumnIndex, _settings.LastSortDirection);
        }

        protected override void OnDeactivate(bool close)
        {
            _settings.WindowSelectionDialogHeight = DialogHeight;
            _settings.WindowSelectionDialogWidth = DialogWidth;
            _settings.NameColumnWidth = NameColumnWidth;

            var t = _columnControl?.Save();
            if (t != null)
            {
                _settings.LastSortDirection = t.Item2;
                _settings.LastSelectedColumnIndex = t.Item1;
            }
            base.OnDeactivate(close);
        }

        private bool CanActivateLayoutItem()
        {
            return SelectedLayoutItems.Count == 1 && SelectedLayoutItems[0] != null;
        }

        private void ActivateLayoutItem()
        {
            InternalActivate(SelectedLayoutItems.FirstOrDefault());
            TryClose(true);
        }

        private bool CanOkClick()
        {
            return true;
        }

        private void OkClick()
        {
            if (SelectedLayoutItems != null)
                InternalActivate(SelectedLayoutItems.FirstOrDefault());
            TryClose(true);
        }

        private bool CanDoubleClick(object obj)
        {
            return obj is ILayoutItemBase;
        }

        private void DoubleClick(object o)
        {
            InternalActivate((ILayoutItemBase)o);
            TryClose(true);
        }

        private void InternalActivate(ILayoutItemBase layoutItemBase)
        {
            if (layoutItemBase is ITool tool)
                _dockingHostViewModel.ShowTool(tool);
            else if (layoutItemBase is ILayoutItem layoutItem)
                _dockingHostViewModel.OpenLayoutItem(layoutItem);
        }

        private bool CanCloseLayoutItem()
        {
            return SelectedLayoutItems.Count > 0 && SelectedLayoutItems.All(x => x != null);
        }

        private void CloseLayoutItem()
        {
            var messageBuilder = new MessageBuilder(WindowOperation.Close);
            foreach (var selectedItem in SelectedLayoutItems.ToList())
            {
                var flag = true;
                if (selectedItem is ITool tool)
                {
                    _dockingHostViewModel.HideTool(tool, false);
                    messageBuilder.AddMessage(tool);
                }
                else if (selectedItem is ILayoutItem layoutItem)
                    flag = _dockingHostViewModel.CloseLayoutItem(layoutItem);

                if (flag)
                {
                    OpenLayoutItems.Remove(selectedItem);
                    messageBuilder.AddMessage(selectedItem);
                }
            }
            MessageText = messageBuilder.GetMessage();
            SelectedLayoutItems.Clear();
            SelectedLayoutItems.Add(OpenLayoutItems.FirstOrDefault());
        }

        private bool CanSaveLayoutItem()
        {
            return SelectedLayoutItems.Any(x =>
                x is IEditor editor && editor.Document is IStorableFile storableFile && storableFile.IsDirty);
        }

        private void SaveLayoutItem()
        {
            var messageBuilder = new MessageBuilder(WindowOperation.Close);
            foreach (var layoutItemBase in SelectedLayoutItems.Where(x =>
                x is IEditor editor && editor.Document is IStorableFile storableFile && storableFile.IsDirty))
            {
                ((IEditor)layoutItemBase).SaveFile();
                messageBuilder.AddMessage(layoutItemBase);

            }
            MessageText = messageBuilder.GetMessage();
        }

        private class MessageBuilder
        {
            private readonly WindowOperation _operation;

            private readonly IList<string> _messages;

            public MessageBuilder(WindowOperation operation)
            {
                _operation = operation;
                _messages = new List<string>();
            }

            public void AddMessage(IHaveDisplayName layoutItem)
            {
                _messages.Add(layoutItem.DisplayName);
            }

            public string GetMessage()
            {
                if (_messages.Count <= 0)
                    return string.Empty;
                switch (_operation)
                {
                    case WindowOperation.Save:
                        if (_messages.Count == 1)
                            return string.Format(WindowSelectionDialogResources.MessageFileSaved, _messages.FirstOrDefault());
                        return string.Format(WindowSelectionDialogResources.MessageFilesSaved, _messages.Count);
                    case WindowOperation.Close:
                        if (_messages.Count == 1)
                            return string.Format(WindowSelectionDialogResources.MessageWindowClosed, _messages.FirstOrDefault());
                        return string.Format(WindowSelectionDialogResources.MessageWindowsClosed, _messages.Count);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private enum WindowOperation
        {
            Save,
            Close
        }
    }

    public interface IWindowSelectionDialogViewModel
    {
        ObservableCollection<ILayoutItemBase> OpenLayoutItems { get; }

        ObservableCollection<ILayoutItemBase> SelectedLayoutItems { get; set; }

        ICommand SaveCommand { get; }

        ICommand ActivateCommand { get; }

        ICommand CloseCommand { get; }

        ICommand DoubleClickCommand { get; }

        string MessageText { get; set; }
    }

    internal interface IWindowSelectionDialogViewModelInternal
    {
        int DialogWidth { get; set; }

        int DialogHeight { get; set; }

        int NameColumnWidth { get; set; }

        int LastSelectedColumnIndex { get; }

        ListSortDirection LastSortDirection { get; }
    }
}