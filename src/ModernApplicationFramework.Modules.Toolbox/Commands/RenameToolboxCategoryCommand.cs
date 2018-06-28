using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IRenameToolboxCategoryCommand))]
    internal class RenameToolboxCategoryCommand : CommandDefinitionCommand, IRenameToolboxCategoryCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public RenameToolboxCategoryCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxCategory;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}