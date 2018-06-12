using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(RenameToolboxItemCommandDefinition))]
    public class RenameToolboxItemCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        public override string NameUnlocalized => "Rename Item";
        public override string Text => "Rename item";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{03E545BE-2ED2-4396-B641-A9582FFF3452}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public RenameToolboxItemCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox;
            var command = new UICommand(RenameItem, CanRenameItem);
            Command = command;
        }

        private bool CanRenameItem()
        {
            return _toolbox.SelectedNode is IToolboxItem;
        }

        private void RenameItem()
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}
