using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
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
    [Export(typeof(SaveActiveFileCommandDefinition))]
    public class SaveActiveFileCommandDefinition : CommandDefinition
    {
        private string _text;

        private readonly IDockingHostViewModel _dockingHostViewModel;
        public override string NameUnlocalized => "Save active file";
        public override string Text => $"{_text} save";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override UICommand Command { get; }

        [ImportingConstructor]
        public SaveActiveFileCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
            var command = new UICommand(SaveFile, CanSaveFile);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.S, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;

            _dockingHostViewModel.ActiveLayoutItemChanged += _dockingHostViewModel_ActiveLayoutItemChanged;
        }

        private void _dockingHostViewModel_ActiveLayoutItemChanged(object sender, EventArgs e)
        {
            _text = _dockingHostViewModel.ActiveItem?.DisplayName;
        }

        private bool CanSaveFile()
        {
            if (_dockingHostViewModel.ActiveItem == null)
                return false;
            return _dockingHostViewModel.ActiveItem is IEditor editor && editor.Document is IStorableFile;
        }

        private void SaveFile()
        {
            ((IEditor) _dockingHostViewModel.ActiveItem)?.SaveFile();
        }
    }
}
