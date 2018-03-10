using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveActiveFileAsCommandDefinition))]
    public class SaveActiveFileAsCommandDefinition : CommandDefinition
    {
        private string _text;

        private readonly IDockingHostViewModel _dockingHostViewModel;
        public override string NameUnlocalized => "Save Selected Items As...";
        public override string Text => BuildText();

        private string BuildText()
        {
            if (CanSaveFileAs())
                return string.Format(CommandsResources.SaveActiveDocumentAsCommandText, _text);
            return CommandsResources.SaveActiveDocumentAsCommandName;
        }

        public override string ToolTip => Text;

        public override string Name => CommandsResources.SaveActiveDocumentAsCommandName;

        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override UICommand Command { get; }

        [ImportingConstructor]
        public SaveActiveFileAsCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
            var command = new UICommand(SaveFileAs, CanSaveFileAs);
            Command = command;
            _dockingHostViewModel.ActiveLayoutItemChanged += _dockingHostViewModel_ActiveLayoutItemChanged;
        }

        private void _dockingHostViewModel_ActiveLayoutItemChanged(object sender, EventArgs e)
        {
            _text = _dockingHostViewModel.ActiveItem?.DisplayName;
            OnPropertyChanged(nameof(Text));
        }

        private bool CanSaveFileAs()
        {
            if (_dockingHostViewModel.ActiveItem == null)
                return false;
            return _dockingHostViewModel.ActiveItem is IEditor editor && editor.Document is IStorableFile;
        }

        private void SaveFileAs()
        {
            ((IEditor) _dockingHostViewModel.ActiveItem)?.SaveFile(true);
        }
    }
}
