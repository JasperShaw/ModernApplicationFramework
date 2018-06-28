using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IAddCategoryCommand))]
    internal class AddCategoryCommand : CommandDefinitionCommand, IAddCategoryCommand
    {
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public AddCategoryCommand(IToolboxService service)
        {
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var c = new ToolboxCategory();
            c.CreatedCancelled += C_CreatedCancelled;
            c.Created += C_Created;
            _service.AddCategory(c);
        }

        private void C_CreatedCancelled(object sender, EventArgs e)
        {
            if (!(sender is IToolboxCategory category))
                return;

            category.Created -= C_Created;
            category.CreatedCancelled -= C_CreatedCancelled;
            _service.RemoveCategory(category);
        }

        private void C_Created(object sender, EventArgs e)
        {
            if (!(sender is IToolboxNode node))
                return;
            node.Created -= C_Created;
            node.CreatedCancelled -= C_CreatedCancelled;
        }
    }
}