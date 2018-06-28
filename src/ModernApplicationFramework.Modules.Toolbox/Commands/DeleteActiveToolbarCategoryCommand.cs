using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IDeleteActiveToolbarCategoryCommand))]
    internal class DeleteActiveToolbarCategoryCommand : CommandDefinitionCommand, IDeleteActiveToolbarCategoryCommand
    {
        private readonly IToolbox _toolbox;
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public DeleteActiveToolbarCategoryCommand(IToolbox toolbox, IToolboxService service)
        {
            _toolbox = toolbox;
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxCategory &&
                   !Equals(_toolbox.SelectedNode, ToolboxCategory.DefaultCategory);
        }

        protected override void OnExecute(object parameter)
        {
            if (!(_toolbox.SelectedNode is IToolboxCategory category))
                return;
            if (category == ToolboxCategory.DefaultCategory)
                return;
            _service.RemoveCategory(category);
        }
    }
}