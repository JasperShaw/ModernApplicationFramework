using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Dialogs.EditorSelectorDialog;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    internal interface IEditorSelectorViewModelInternal : IScreen
    {
        IEnumerable<EditorListItem> Editors { get; }

        EditorListItem SelectedEditor { get; set; }

        ICommand OkCommand { get; }

        ICommand SetDefaultCommand { get; }
    }
}