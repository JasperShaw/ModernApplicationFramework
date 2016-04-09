﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof (CommandDefinition))]
    public sealed class UndoCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public UndoCommandDefinition()
        {
            Command = new GestureCommandWrapper(Undo, CanUndo, new KeyGesture(Key.Z, ModifierKeys.Control));
        }

        public override string IconId => "UndoIcon";
        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }
        public override Uri IconSource => new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/Undo_16x.xaml", UriKind.RelativeOrAbsolute);

        public string MyText { get; set; }

        public override string Name => "Undo";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanUndo()
        {
            return _shell?.DockingHost.ActiveItem != null &&
                   _shell.DockingHost.ActiveItem.UndoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _shell.DockingHost.ActiveItem.UndoCommand.Execute(null);
        }
    }
}