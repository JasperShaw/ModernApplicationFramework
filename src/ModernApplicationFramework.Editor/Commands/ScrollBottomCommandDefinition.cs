using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollBottomCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollBottomCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollBottom))
        {
        }

        public override string NameUnlocalized => "Scroll Bottom";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{E029B052-577B-4A0B-84AE-D455353D63DF}");
    }
}