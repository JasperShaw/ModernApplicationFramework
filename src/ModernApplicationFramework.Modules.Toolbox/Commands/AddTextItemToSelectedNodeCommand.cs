using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IAddTextItemToSelectedNodeCommand))]
    internal class AddTextItemToSelectedNodeCommand : CommandDefinitionCommand, IAddTextItemToSelectedNodeCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public AddTextItemToSelectedNodeCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (_toolbox.SelectedNode == null)
                return false;
            if (!(parameter is string))
                return false;
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is IDataObject data))
                return;
            var item = ToolboxItem.CreateTextItem(data);

            if (_toolbox.SelectedNode is IToolboxItem selectedItem)
            {
                var category = selectedItem.Parent;
                var index = category.Items.IndexOf(selectedItem);
                category.Items.Insert(index +1, item);
            }
            else if (_toolbox.SelectedNode is IToolboxCategory selectedCategory)
            {
                selectedCategory.Items.Insert(0, item);
            }
        }
    }
}
