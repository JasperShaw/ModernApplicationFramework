using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class RedoCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public RedoCommandDefinition()
        {
            Command = new GestureCommandWrapper(Redo, CanRedo, new KeyGesture(Key.Y, ModifierKeys.Control));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "RedoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/Redo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Redo";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanRedo()
        {
            return _shell?.DockingHost.ActiveItem != null &&
                   _shell.DockingHost.ActiveItem.UndoRedoManager.RedoStack.Any();
        }

        private void Redo()
        {
            _shell.DockingHost.ActiveItem.RedoCommand.Execute(null);
        }
    }
}