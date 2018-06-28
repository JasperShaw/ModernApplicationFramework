using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ISaveActiveFileAsCommand))]
    internal class SaveActiveFileAsCommand : CommandDefinitionCommand, ISaveActiveFileAsCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public SaveActiveFileAsCommand(IDockingHostViewModel dockingHostViewModel)
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
            ((IEditor)_dockingHostViewModel.ActiveItem)?.SaveFile(true);
        }
    }
}