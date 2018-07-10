using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IAddItemToSelectedNodeCommand))]
    internal class AddItemToSelectedNodeCommand : CommandDefinitionCommand, IAddItemToSelectedNodeCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public AddItemToSelectedNodeCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {

            if (!(parameter is IDataObject dataObject))
                return;

            IToolboxItem item;
            if (dataObject.GetFormats().Any(x => x == DataFormats.Text))
            {
                item = ToolboxItem.CreateTextItem(new ToolboxItemData(DataFormats.Text, dataObject.GetData(DataFormats.Text)));
            }
            else if (dataObject.GetDataPresent(ToolboxItemDataFormats.DataSource))
            {
                try
                {
                    var dataSource = dataObject.GetData(ToolboxItemDataFormats.DataSource) as ToolboxItemDefinition;
                    item = new ToolboxItem(dataSource);
                }
                catch (ExternalException)
                {
                    return;
                }            
            }
            else
                return;

            if (_toolbox.SelectedNode is IToolboxItem selectedItem)
            {
                var category = selectedItem.Parent;
                var index = category.Items.IndexOf(selectedItem);
                category.Items.Insert(index + 1, item);
            }
            else if (_toolbox.SelectedNode is IToolboxCategory selectedCategory)
            {
                selectedCategory.Items.Insert(0, item);
            }
        }
    }
}
