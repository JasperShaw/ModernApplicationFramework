using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ToggleCaseCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ToggleCaseCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ToggleCase))
        {
        }

        public override string NameUnlocalized => "Toggle Case";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{BF7254A1-E00A-41F9-AA11-8F48C231492C}");
    }
}