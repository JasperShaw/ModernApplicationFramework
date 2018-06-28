using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IOpenToolboxCommand))]
    internal class OpenToolboxCommand : CommandDefinitionCommand, IOpenToolboxCommand
    {
        private readonly IDockingHostViewModel _hostViewModel;

        [ImportingConstructor]
        public OpenToolboxCommand(IDockingHostViewModel hostViewModel)
        {
            _hostViewModel = hostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            _hostViewModel.ShowTool<IToolbox>();
        }
    }
}