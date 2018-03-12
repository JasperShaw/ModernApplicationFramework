using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Controls.WindowSelectionDialog
{
    [Export(typeof(IWindowSelectionDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class WindowSelectionDialogViewModel : Screen, IWindowSelectionDialogViewModel
    { 
        private readonly IDockingHostViewModel _dockingHostViewModel;
        private ILayoutItemBase _selectedLayoutItem;

        public ObservableCollection<ILayoutItemBase> OpenLayoutItems { get; }

        public ILayoutItemBase SelectedLayoutItem
        {
            get => _selectedLayoutItem;
            set
            {
                if (Equals(value, _selectedLayoutItem)) return;
                _selectedLayoutItem = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand SaveCommand => new UICommand(SaveLayoutItem, CanSaveLayoutItem);

        public ICommand CloseCommand => new UICommand(CloseLayoutItem, CanCloseLayoutItem);

        public ICommand ActivateCommand => new UICommand(ActivateLayoutItem, CanActivateLayoutItem);

        [ImportingConstructor]
        public WindowSelectionDialogViewModel(IDockingHostViewModel dockingHostViewModel)
        {
            DisplayName = WindowSelectionDialogResources.DialogTitle;
            _dockingHostViewModel = dockingHostViewModel;
            OpenLayoutItems =
                new ObservableCollection<ILayoutItemBase>(_dockingHostViewModel.DockingHostView
                    .AllOpenLayoutItemsAsDocuments);
            SelectedLayoutItem = OpenLayoutItems.FirstOrDefault(x => x.IsActive);
        }

        private static bool CanActivateLayoutItem()
        {
            return true;
        }

        private void ActivateLayoutItem()
        {
            TryClose(true);
        }

        private static bool CanCloseLayoutItem()
        {
            return true;
        }

        private void CloseLayoutItem()
        {
            var flag = true;
            if (SelectedLayoutItem is ITool tool)
                _dockingHostViewModel.HideTool(tool, false);
            else if (SelectedLayoutItem is ILayoutItem layoutItem)
                flag = _dockingHostViewModel.CloseLayoutItem(layoutItem);
            if (flag)
                OpenLayoutItems.Remove(SelectedLayoutItem);
        }

        private bool CanSaveLayoutItem()
        {
            if (SelectedLayoutItem is IEditor editor && editor.Document is IStorableFile storableFile &&
                !storableFile.IsDirty)
                return true;
            return false;
        }

        private void SaveLayoutItem()
        {
            if (SelectedLayoutItem is IEditor editor)
                editor.SaveFile();
        }


        private void ChangeSorting(IComparer sortOrder)
        {
            if (OpenLayoutItems == null)
                return;
            if (!(CollectionViewSource.GetDefaultView(OpenLayoutItems) is ListCollectionView defaultView) || Equals(defaultView.CustomSort, sortOrder))
                return;
            defaultView.CustomSort = sortOrder;
            defaultView.Refresh();
        }

    }

    public interface IWindowSelectionDialogViewModel
    {
        ObservableCollection<ILayoutItemBase> OpenLayoutItems { get; }

        ILayoutItemBase SelectedLayoutItem { get; set; }

        ICommand SaveCommand { get; }

        ICommand ActivateCommand { get; }

        ICommand CloseCommand { get; }
    }
}
