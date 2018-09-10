using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollLeftCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollLeftCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollLeft))
        {
        }

        public override string NameUnlocalized => "Scroll Left";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{46F50969-8349-4F6F-BCF6-10ED98506007}");
    }
}