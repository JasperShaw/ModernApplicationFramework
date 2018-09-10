using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollPageDownCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollPageDownCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollPageDown))
        {
        }

        public override string NameUnlocalized => "Scroll Page Down";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{F117183B-41A1-48B0-9ABA-3AB7A5EB9061}");
    }
}