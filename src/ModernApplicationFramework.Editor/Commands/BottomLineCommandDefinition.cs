using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class BottomLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public BottomLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.BottomLine))
        {
        }

        public override string NameUnlocalized => "Bottom Line";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{193E06E3-A442-418E-A3F4-F0D549B9F0BD}");
    }
}