using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Settings.SettingsDialog.ViewModels;

namespace ModernApplicationFramework.Extended.Demo.Modules.CommandOverride
{
    [Export(typeof(IOpenSettingsCommand))]
    internal class OpenSettingsCommand : CommandDefinitionCommand, IOpenSettingsCommand
    {
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public OpenSettingsCommand(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        protected override bool CanExecute()
        {
            return true;
        }

        protected override void Execute()
        {
            MessageBox.Show("Message first");
            _windowManager.ShowDialog(IoC.Get<SettingsWindowViewModel>());
        }
    }
}
