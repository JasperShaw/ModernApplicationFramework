using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof (CommandDefinition))]
    public sealed class RedoCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public RedoCommandDefinition()
        {
            Command = new GestureCommandWrapper(Test, CanTest, new KeyGesture(Key.Y, ModifierKeys.Control));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }
        public override Uri IconSource { get; }

        public override string Name => "Redo";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanTest()
        {
            return _shell?.DockingHost.ActiveItem != null &&
                   _shell.DockingHost.ActiveItem.UndoRedoManager.RedoStack.Any();
        }

        private void Test()
        {
            _shell.DockingHost.ActiveItem.RedoCommand.Execute(null);
        }
    }
}