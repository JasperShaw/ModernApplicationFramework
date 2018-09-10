using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollPageUpCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollPageUpCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollPageUp))
        {
        }

        public override string NameUnlocalized => "Scroll Page Up";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{EB22C6F5-BF66-4F43-AE66-BCDB6B77B5A3}");
    }
}