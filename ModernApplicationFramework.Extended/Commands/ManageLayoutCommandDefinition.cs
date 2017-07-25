using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Extended.Core.LayoutManagement;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ManageLayoutCommandDefinition))]
    public sealed class ManageLayoutCommandDefinition : CommandDefinition
    {
        private readonly ILayoutManager _layoutManager;
        public override string Name => Commands_Resources.ManageLayoutCommandDefinition_Name;
        public override string Text => Commands_Resources.ManageLayoutCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ManageLayoutCommandDefinition(ILayoutManager layoutManager)
        {
            _layoutManager = layoutManager;

            var command = new UICommand(Manage, CanManage);
            Command = command;
        }

        private bool CanManage()
        {
            return true;
        }

        private void Manage()
        {
            _layoutManager.ManageWindowLayouts();
        }
    }
}
