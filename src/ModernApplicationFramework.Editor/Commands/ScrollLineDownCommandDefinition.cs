using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollLineDownCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollLineDownCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollDown))
        {
        }

        public override string NameUnlocalized => "Scroll Line Down";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{7093BD77-0579-4B9A-AA15-1D77B218B365}");
    }
}