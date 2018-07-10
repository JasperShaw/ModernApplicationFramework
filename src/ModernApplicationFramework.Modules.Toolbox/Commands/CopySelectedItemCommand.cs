using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Items;

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

            IDataObject dataObject;
            if (item.DataSource.Data.Format == DataFormats.Text)
                dataObject = new DataObject(DataFormats.Text, item.DataSource.Data.Data);
            else
            {
                var dataSource = new ToolboxItemDefinition(item.DataSource);
                if (!IsSerializable(dataSource))
                    return;
                dataObject = new DataObject(ToolboxItemDataFormats.DataSource, dataSource);
            }
            Clipboard.SetDataObject(dataObject, true);

            if ((bool)parameter)
            {
                var result = RemoveItemDilalog.AskUserForRemove(item);
                if (result == MessageBoxResult.Cancel)
                    return;
                var category = item.Parent;
                category.Items.Remove(item);
                _toolbox.SelectedNode = category;
            }
        }

        private static bool IsSerializable(object obj)
        {
            var mem = new System.IO.MemoryStream();
            var bin = new BinaryFormatter();
            try
            {
                bin.Serialize(mem, obj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
