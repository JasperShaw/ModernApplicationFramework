using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal sealed class ManageLayoutsViewModel : ObservableObject
    {
        private LayoutItemViewModel _selectedLayout;

        public ICommand RenameLayoutCommand { get; }

        public ICommand DeleteLayoutCommand { get; }

        public ICommand MoveLayoutUpCommand { get; }

        public ICommand MoveLayoutDownCommand { get; }

        public ObservableCollection<LayoutItemViewModel> Layouts { get; }

        public LayoutItemViewModel SelectedLayout
        {
            get => _selectedLayout;
            set
            {
                var flag = _selectedLayout == null != (value == null);
                if (!(SetProperty(ref _selectedLayout, value) & flag))
                    return;
                ((ObjectCommand)RenameLayoutCommand).RaiseCanExecuteChanged();
                ((ObjectCommand)DeleteLayoutCommand).RaiseCanExecuteChanged();
                ((ObjectCommand)MoveLayoutUpCommand).RaiseCanExecuteChanged();
                ((ObjectCommand)MoveLayoutDownCommand).RaiseCanExecuteChanged();
            }
        }

        internal ManageLayoutsViewModel(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection, ILayoutManagementUserManageInput userInput)
        {
            Validate.IsNotNull(layoutKeyInfoCollection, "layoutKeyInfoCollection");
            RenameLayoutCommand = new ObjectCommand(OnRenameLayoutCommandExecuted, CanExecuteLayoutCommand);
            DeleteLayoutCommand = new ObjectCommand(OnDeleteLayoutCommandExecuted, CanExecuteLayoutCommand);
            MoveLayoutUpCommand = new ObjectCommand(OnMoveLayoutUpExecuted, CanExecuteMoveUpCommand);
            MoveLayoutDownCommand = new ObjectCommand(OnMoveLayoutDownExecuted, CanExecuteMoveDownCommand);
            Layouts = new ObservableCollection<LayoutItemViewModel>(layoutKeyInfoCollection.Select(kvp => new LayoutItemViewModel(kvp.Key, kvp.Value)).OrderBy(lvm => lvm.Position).ThenBy(lvm => lvm.Name));
            FixLayoutPositions();
            SelectedLayout = Layouts.FirstOrDefault();
        }

        private void MoveSelectedLayout(int swapOffset)
        {
            if (swapOffset != 1 && swapOffset != -1)
                return;
            var selectedLayout = SelectedLayout;
            if (selectedLayout == null)
                return;
            var oldIndex = Layouts.IndexOf(selectedLayout);
            if (oldIndex < 0)
                return;
            var newIndex = oldIndex + swapOffset;
            if (newIndex < 0 || newIndex >= Layouts.Count)
                return;
            var layout = Layouts[newIndex];
            Layouts.Move(oldIndex, newIndex);
            layout.Position = oldIndex;
            selectedLayout.Position = newIndex;
        }

        private void OnMoveLayoutUpExecuted(object parameter)
        {
            MoveSelectedLayout(-1);
        }

        private void OnMoveLayoutDownExecuted(object parameter)
        {
            MoveSelectedLayout(1);
        }

        private void OnDeleteLayoutCommandExecuted(object parameter)
        {
            var selectedLayout = SelectedLayout;
            if (selectedLayout == null || !LayoutManagementDialogUserInput.GetDeleteLayoutConfirmation(selectedLayout.Name))
                return;
            var index = Layouts.IndexOf(selectedLayout);
            if (index == -1)
                return;
            RemoveLayoutAt(index);
        }

        private void RemoveLayoutAt(int index)
        {
            if (index < 0 || index >= Layouts.Count)
                return;
            var flag = SelectedLayout == Layouts[index];
            Layouts.RemoveAt(index);
            FixLayoutPositions();
            if (!flag)
                return;
            SelectedLayout = Layouts.Count > index ? Layouts[index] : Layouts.LastOrDefault();
        }

        private void OnRenameLayoutCommandExecuted(object parameter)
        {
            var selectedLayout = SelectedLayout;
            if (selectedLayout == null || !LayoutManagementDialogUserInput.GetRenamedLayoutName(selectedLayout.Name, HandleLayoutNameConflict, out var layoutName))
                return;
            selectedLayout.Name = LayoutManagementUtilities.NormalizeName(layoutName);
        }

        private bool HandleLayoutNameConflict(string name)
        {
            name = LayoutManagementUtilities.NormalizeName(name);
            var num = Layouts.IndexOf(SelectedLayout);
            if (num == -1)
                return false;
            if (!LayoutManagementUtilities.IsUniqueName(name, Layouts.Select(layout => layout.Name), out var conflictingIndex) && conflictingIndex != num)
            {
                if (!LayoutManagementDialogUserInput.GetReplaceLayoutConfirmation(name))
                    return false;
                RemoveLayoutAt(conflictingIndex);
            }
            return true;
        }

        private bool CanExecuteLayoutCommand(object parameter)
        {
            return SelectedLayout != null;
        }

        private bool CanExecuteMoveUpCommand(object parameter)
        {
            if (CanExecuteLayoutCommand(parameter))
                return SelectedLayout != Layouts.FirstOrDefault();
            return false;
        }

        private bool CanExecuteMoveDownCommand(object parameter)
        {
            if (CanExecuteLayoutCommand(parameter))
                return SelectedLayout != Layouts.LastOrDefault();
            return false;
        }

        private void FixLayoutPositions()
        {
            for (var index = 0; index < Layouts.Count; ++index)
                Layouts[index].Position = index;
        }
    }
}
