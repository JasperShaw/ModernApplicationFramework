using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOpenOutputToolCommand))]
    internal class OpenOutputToolCommand : CommandDefinitionCommand, IOpenOutputToolCommand
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        protected override bool OnCanExecute(object parameter)
        {
            return _shell != null;
        }

        protected override void OnExecute(object parameter)
        {
            _shell.DockingHost.ShowTool<IOutputPane>();
        }
    }
}