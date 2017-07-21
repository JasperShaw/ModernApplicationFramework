using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of the command to open the command bar customize dialog
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" />
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CustomizeMenuCommandDefinition))]
    public sealed class CustomizeMenuCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }
        public override string Name => Text;
        public override string Text => CommandBarResources.CustomizeMenuCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override string ShortcutText => null;

        public CustomizeMenuCommandDefinition()
        {
            Command = new Command(OpenCustomizeDialog, () => true);
        }

        private void OpenCustomizeDialog()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new CustomizeDialogViewModel();
            windowManager.ShowDialog(customizeDialog);
        }
    }
}