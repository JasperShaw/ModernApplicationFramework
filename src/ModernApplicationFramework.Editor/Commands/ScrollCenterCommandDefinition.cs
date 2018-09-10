using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollCenterCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollCenterCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollCenter))
        {
        }

        public override string NameUnlocalized => "Scroll Center";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{F7D9DF14-DBBE-411A-9F32-360898D1F5B7}");
    }
}