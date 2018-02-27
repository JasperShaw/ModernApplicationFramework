using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Controls.EditorSelectorDialog
{
    [Export(typeof(IEditorSelectorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class EditorSelectorViewModel : Screen, IEditorSelectorViewModel, IEditorSelectorViewModelInternal
    {
        private IEditor _selectedEditor;
        public IEnumerable<IEditor> Editors { get; }

        public IEditor SelectedEditor
        {
            get => _selectedEditor;
            set
            {
                if (Equals(value, _selectedEditor)) return;
                _selectedEditor = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand OkCommand => new UICommand(CreateResult, CanCreateResult);

        public IExtensionDefinition TargetExtension { get; set; }
        public IEditor Result { get; private set; }

        [ImportingConstructor]
        public EditorSelectorViewModel([ImportMany] IEditor[] editors)
        {
            Editors = editors;
        }

        private void CreateResult()
        {
            Result = SelectedEditor;
            TryClose(true);
        }

        private bool CanCreateResult()
        {
            return SelectedEditor != null;
        }
    }

    public interface IEditorSelectorViewModel
    {
        IExtensionDefinition TargetExtension { get; set; }

        IEditor Result { get; }
    }

    internal interface IEditorSelectorViewModelInternal : IScreen
    {
        IEnumerable<IEditor> Editors { get; }

        IEditor SelectedEditor { get; set; }

        ICommand OkCommand { get; }
    }
}
