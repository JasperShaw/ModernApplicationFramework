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
    [Export(typeof(RenameToolboxCategoryCommandDefinition))]
    public class RenameToolboxCategoryCommandDefinition : CommandDefinition<IRenameToolboxCategoryCommand>
    {
        public override string NameUnlocalized => "Rename Category";
        public override string Text => "Rename Category";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{0524D1D9-DF40-4D62-85DB-966AED7F8C35}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IRenameToolboxCategoryCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IRenameToolboxCategoryCommand))]
    internal class RenameToolboxCategoryCommand : CommandDefinitionCommand, IRenameToolboxCategoryCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public RenameToolboxCategoryCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedNode is IToolboxCategory;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}
