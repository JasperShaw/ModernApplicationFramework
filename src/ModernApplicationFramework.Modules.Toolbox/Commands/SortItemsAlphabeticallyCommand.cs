using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(ISortItemsAlphabeticallyCommand))]
    internal class SortItemsAlphabeticallyCommand : CommandDefinitionCommand, ISortItemsAlphabeticallyCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public SortItemsAlphabeticallyCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedCategory != null && _toolbox.SelectedCategory.Items.Count >= 2;
        }

        protected override void OnExecute(object parameter)
        {
            if (_toolbox.SelectedCategory == null)
                return;
            var selectedNode = _toolbox.SelectedNode;
            var oldLayout = _toolbox.SelectedCategory.Items.ToList();
            var newLayout = oldLayout.OrderBy(x => x.Name);
            _toolbox.SelectedCategory.Items.Clear();
            _toolbox.SelectedCategory.Items.AddRange(newLayout);
            _toolbox.SelectedNode = selectedNode;
        }
    }
}