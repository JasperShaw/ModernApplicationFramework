using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollRightCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollRightCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollRight))
        {
        }

        public override string NameUnlocalized => "Scroll Right";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{2AAB5415-9223-4977-9F36-9EC3729AD5E5}");
    }
}