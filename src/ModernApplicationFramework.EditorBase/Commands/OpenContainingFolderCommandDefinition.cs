using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenContainingFolderCommandDefinition : CommandDefinition
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        public override string NameUnlocalized => "Open Containing Folder";
        public override string Text => CommandsResources.OpenContainingFolderCommandText;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{1E11050B-F441-4C18-8A0E-B6C46D4265DE}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public OpenContainingFolderCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
            var command = new UICommand(CopyFullPath, CanCopyFullPath);
            Command = command;
        }

        private bool CanCopyFullPath()
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.Path);
        }

        private void CopyFullPath()
        {
            Process.Start(((IEditor) _dockingHostViewModel.ActiveItem)?.Document.Path);
        }
    }
}
