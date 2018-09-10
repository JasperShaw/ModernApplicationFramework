using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class BottomLineExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public BottomLineExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.BottomLineExt))
        {
        }

        public override string NameUnlocalized => "Bottom Line Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{E8AACE3C-35BF-4E9E-B0AF-69F9128E8DA4}");
    }
}