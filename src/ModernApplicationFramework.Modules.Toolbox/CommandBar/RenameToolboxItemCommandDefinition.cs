using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(RenameToolboxItemCommandDefinition))]
    public class RenameToolboxItemCommandDefinition : CommandDefinition
    {
        private readonly ToolboxViewModel _toolbox;
        public override string NameUnlocalized => "Rename";
        public override string Text => "Rename";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{0524D1D9-DF40-4D62-85DB-966AED7F8C35}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override UICommand Command { get; }

        [ImportingConstructor]
        public RenameToolboxItemCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;

            var command = new UICommand(RenameItem, CanRenameItem);
            Command = command;
        }

        private bool CanRenameItem()
        {
            return _toolbox.SelectedNode != null;
        }

        private void RenameItem()
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}
