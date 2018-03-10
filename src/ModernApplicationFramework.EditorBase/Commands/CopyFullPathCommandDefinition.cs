using System;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyFullPathCommandDefinition : CommandDefinition
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        public override string NameUnlocalized => "Copy Full Path";
        public override string Text => CommandsResources.CopyFullPathCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{1F2CAB1F-3624-4D2E-9855-4CD6F62F7B13}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override UICommand Command { get; }

        [ImportingConstructor]
        public CopyFullPathCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
            var command = new UICommand(CopyFullPath, CanCopyFullPath);
            Command = command;
        }

        private bool CanCopyFullPath()
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.FileName);
        }

        private void CopyFullPath()
        {
            Clipboard.SetText(((IEditor)_dockingHostViewModel.ActiveItem)?.Document.FullFilePath);
        }
    }
}
