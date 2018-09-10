using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class CapitalizeCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public CapitalizeCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.Capitalize))
        {
        }

        public override string NameUnlocalized => "Capitalize";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{79462A49-74EB-47A9-B250-095BA0A119C1}");
    }
}