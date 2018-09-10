using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollTopCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollTopCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollTop))
        {
        }

        public override string NameUnlocalized => "Scroll Top";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{285F36E0-F36C-4EE6-9475-664916C070D1}");
    }
}