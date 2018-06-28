using System.ComponentModel.Composition;
using System.Diagnostics;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(IOpenContainingFolderCommand))]
    internal class OpenContainingFolderCommand : CommandDefinitionCommand, IOpenContainingFolderCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public OpenContainingFolderCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.Path);
        }

        protected override void OnExecute(object parameter)
        {
            Process.Start(((IEditor)_dockingHostViewModel.ActiveItem)?.Document.Path);
        }
    }
}