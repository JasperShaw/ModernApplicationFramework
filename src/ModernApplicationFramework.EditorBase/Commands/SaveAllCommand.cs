using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(ISaveAllCommand))]
    internal class SaveAllCommand : CommandDefinitionCommand, ISaveAllCommand
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        [ImportingConstructor]
        public SaveAllCommand(IDockingHostViewModel dockingHostViewModel)
        {
            _dockingHostViewModel = dockingHostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (_dockingHostViewModel.LayoutItems == null || _dockingHostViewModel.LayoutItems.Count == 0)
                return false;
            return _dockingHostViewModel.LayoutItems.Any(x => x is IEditor editor && editor.Document is IStorableFile);
        }

        protected override void OnExecute(object parameter)
        {
            foreach (var editor in _dockingHostViewModel.LayoutItems.OfType<IEditor>().Where(x => x.Document is IStorableFile))
            {
                editor.SaveFile();
            }
        }
    }
}