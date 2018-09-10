using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class TopLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TopLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TopLine))
        {
        }

        public override string NameUnlocalized => "Top Line";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{6C7DE4FD-5E92-4B1E-B8C6-A5429CF2590E}");
    }
}