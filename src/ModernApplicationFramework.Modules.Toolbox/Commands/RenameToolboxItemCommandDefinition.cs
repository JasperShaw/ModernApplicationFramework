using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(RenameToolboxItemCommandDefinition))]
    public class RenameToolboxItemCommandDefinition : CommandDefinition<IRenameToolboxItemCommand>
    {
        public override string NameUnlocalized => "Rename Item";
        public override string Text => "Rename item";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{03E545BE-2ED2-4396-B641-A9582FFF3452}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IRenameToolboxItemCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IRenameToolboxItemCommand))]
    internal class RenameToolboxItemCommand : CommandDefinitionCommand, IRenameToolboxItemCommand
    {
        private readonly IToolbox _toolbox;


        [ImportingConstructor]
        public RenameToolboxItemCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxItem;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}
