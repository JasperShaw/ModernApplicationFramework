using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(ICopySelectedItemCommand))]
    public class CopySelectedItemCommand : CommandDefinitionCommand, ICopySelectedItemCommand
    {

        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public CopySelectedItemCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            if (!(_toolbox.SelectedNode is IToolboxItem item))
                return;
            Clipboard.SetDataObject(item.Data, true);

            if ((bool)parameter)
            {
                var result = RemoveItemDilalog.AskUserForRemove(item);
                if (result == MessageBoxResult.No)
                    return;
                var category = item.Parent;
                category.Items.Remove(item);
                _toolbox.SelectedNode = category;
            }
        }
    }
}
