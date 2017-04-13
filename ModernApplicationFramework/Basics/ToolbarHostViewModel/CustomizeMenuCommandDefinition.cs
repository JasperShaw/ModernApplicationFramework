using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.ToolbarHostViewModel
{
    [Export(typeof(DefinitionBase))]
    public sealed class CustomizeMenuCommandDefinition : CommandDefinition
    {
        public CustomizeMenuCommandDefinition()
        {
            Command = new CommandBase.Command(OpenCustomizeDialog, () => true);
        }

        private void OpenCustomizeDialog()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new CustomizeDialogViewModel();
            windowManager.ShowDialog(customizeDialog);
        }

        public override ICommand Command { get; }
        public override string Name => null;
        public override string Text => "&Customize...";
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override string ShortcutText => null;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
    }
}
