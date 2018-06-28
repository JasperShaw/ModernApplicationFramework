using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ICopyFullPathCommand))]
    internal class CopyFullPathCommand : CommandDefinitionCommand, ICopyFullPathCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public CopyFullPathCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _dockingHostViewModel.ActiveItem is IEditor editor && !string.IsNullOrEmpty(editor.Document.FileName);
        }

        protected override void OnExecute(object parameter)
        {
            Clipboard.SetText(((IEditor)_dockingHostViewModel.ActiveItem)?.Document.FullFilePath);
        }
    }
}