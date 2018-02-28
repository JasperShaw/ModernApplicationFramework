using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.Interfaces.Settings;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Controls.EditorSelectorDialog
{
    [Export(typeof(IEditorSelectorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class EditorSelectorViewModel : Screen, IEditorSelectorViewModel, IEditorSelectorViewModelInternal
    {
        private readonly IEditorFileAssociationSettings _settings;
        private EditorListItem _selectedEditor;
        private IExtensionDefinition _targetExtension;
        public IEnumerable<EditorListItem> Editors { get; }

        public EditorListItem SelectedEditor
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
        public ICommand SetDefaultCommand => new UICommand(SetActiveItemAsDefault, CanSetActiveItemAsDefault);

        public IExtensionDefinition TargetExtension
        {
            get => _targetExtension;
            set
            {
                _targetExtension = value;
                GetDefaultEditor(value);
            }
        }

        public IEditor Result { get; private set; }

        [ImportingConstructor]
        public EditorSelectorViewModel([ImportMany] IEditor[] editors, IEditorFileAssociationSettings settings)
        {
            DisplayName = EditorSelectorResources.WindowTitle;
            _settings = settings;
            var items = editors.Select(editor => new EditorListItem(editor)).ToList();
            Editors = new List<EditorListItem>(items);
        }

        private void CreateResult()
        {
            Result = SelectedEditor.Editor;
            TryClose(true);
        }

        private bool CanCreateResult()
        {
            return SelectedEditor != null;
        }

        private void GetDefaultEditor(IExtensionDefinition file)
        {
            var editor = _settings.GetAssociatedEditor((ISupportedFileDefinition) file);
            var item = Editors.FirstOrDefault(x => x.Editor.EditorId == editor.EditorId);
            if (item == null)
                return;
            item.IsDefault = true;
            SelectedEditor = item;
        }

        private void SetActiveItemAsDefault()
        {
            var item = Editors.FirstOrDefault(x => x.IsDefault);
            if (item == null)
                return;
            if (item == SelectedEditor)
                return;
            item.IsDefault = false;
            _settings.CreateAssociation(SelectedEditor.Editor, (ISupportedFileDefinition) TargetExtension);
            SelectedEditor.IsDefault = true;
        }

        private bool CanSetActiveItemAsDefault()
        {
            return SelectedEditor != null;
        }
    }
}
