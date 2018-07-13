using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IAddItemCommand))]
    internal class AddItemCommand : CommandDefinitionCommand, IAddItemCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var window = IoC.Get<ChooseItemsDialogViewModel>();
            var dataSource = GetDataSource();
            window.DataSource = dataSource;
            IoC.Get<IWindowManager>().ShowDialog(window);
        }

        private static ChooseItemsDataSource GetDataSource()
        {
            var factory = IoC.Get<IWaitDialogFactory>();
            using (factory?.StartWaitDialog("Loading", null, TimeSpan.FromSeconds(0.0)))
                return new ChooseItemsDataSource();
        }
    }
}