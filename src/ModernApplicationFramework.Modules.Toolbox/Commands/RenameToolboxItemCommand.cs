using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IRenameToolboxItemCommand))]
    internal class RenameToolboxItemCommand : CommandDefinitionCommand, IRenameToolboxItemCommand
    {
        private readonly IToolbox _toolbox;


        [ImportingConstructor]
        public RenameToolboxItemCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxItem;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}