using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IToggleShowAllItemsCommand))]
    internal class ToggleShowAllItemsCommand : CommandDefinitionCommand, IToggleShowAllItemsCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public ToggleShowAllItemsCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.ShowAllItems = !_toolbox.ShowAllItems;
            if (_toolbox.ShowAllItems)
                Checked = true;
            else
                Checked = false;
        }
    }
}