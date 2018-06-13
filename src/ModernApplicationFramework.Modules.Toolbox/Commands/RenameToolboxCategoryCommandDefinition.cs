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
    [Export(typeof(RenameToolboxCategoryCommandDefinition))]
    public class RenameToolboxCategoryCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        public override string NameUnlocalized => "Rename Category";
        public override string Text => "Rename Category";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{0524D1D9-DF40-4D62-85DB-966AED7F8C35}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public RenameToolboxCategoryCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox;
            var command = new UICommand(RenameCategory, CanRenameCategory);
            Command = command;
        }

        private bool CanRenameCategory()
        {
            return _toolbox.SelectedNode is IToolboxCategory;
        }

        private void RenameCategory()
        {
            _toolbox.SelectedNode.EnterRenameMode();
        }
    }
}
