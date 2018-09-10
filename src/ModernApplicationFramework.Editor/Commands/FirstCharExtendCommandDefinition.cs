using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class FirstCharExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public FirstCharExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.FirstCharExt))
        {
        }

        public override string NameUnlocalized => "FirstChar Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{7713077A-F0A1-4D4A-A09B-AFDF90EC06DE}");
    }
}