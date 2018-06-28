using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ISaveActiveFileCommand))]
    internal class SaveActiveFileCommand : CommandDefinitionCommand, ISaveActiveFileCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public SaveActiveFileCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }
        protected override bool OnCanExecute(object parameter)
        {
            if (_dockingHostViewModel.ActiveItem == null)
                return false;
            return _dockingHostViewModel.ActiveItem is IEditor editor && editor.Document is IStorableFile;
        }

        protected override void OnExecute(object parameter)
        {
            ((IEditor)_dockingHostViewModel.ActiveItem)?.SaveFile();
        }
    }
}